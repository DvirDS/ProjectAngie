using UnityEngine;

public enum EnemyState { Idle, Patrol, Alert, Chase, Return }

public class EnemyAI : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private EnemySO data;

    [Header("State Settings")]
    public EnemyState currentState = EnemyState.Patrol;

    [Header("Movement Type")]
    [Tooltip("Disables gravity — enemy floats at its starting height.")]
    [SerializeField] private bool canFly = false;

    [Header("Patrol Points")]
    public Transform[] waypoints;

    [Header("References")]
    [Header("References")]
    [SerializeField] private SpriteRenderer alertBubble;
    [SerializeField] private LayerMask detectionLayers;

    private Rigidbody2D rb;
    private Transform player;
    private PlayerStealth playerStealth; 
    private bool isFacingRight = true;
    private Vector2 startPosition;
    private int currentWaypointIndex = 0;

    private float alertTimer = 0f;
    private float waypointPauseTimer = 0f;
    private bool isWaypointPausing = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError("EnemyAI requires a Rigidbody2D component.");
        if (canFly && rb != null) rb.gravityScale = 0f;
    }

    void Start()
    {
        FindPlayerReference();

        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;

        if (alertBubble != null) alertBubble.gameObject.SetActive(false);

        ChangeState(currentState);
    }

    void Update()
    {
        if (player == null)
        {
            FindPlayerReference();
            if (player == null) return;
        }

        switch (currentState)
        {
            case EnemyState.Idle: UpdateIdle(); break;
            case EnemyState.Patrol: UpdatePatrol(); break;
            case EnemyState.Alert: UpdateAlert(); break;
            case EnemyState.Chase: UpdateChase(); break;
            case EnemyState.Return: UpdateReturn(); break;
        }
    }

    private void UpdateIdle()
    {
        SetVelocityX(0f);
        if (CanSeePlayer()) ChangeState(EnemyState.Alert);
    }

    private void UpdatePatrol()
    {
        if (CanSeePlayer())
        {
            ChangeState(EnemyState.Alert);
            return;
        }

        if (waypoints == null || waypoints.Length == 0)
        {
            SetVelocityX(0f);
            return;
        }

        if (isWaypointPausing)
        {
            SetVelocityX(0f);
            waypointPauseTimer -= Time.deltaTime;
            if (waypointPauseTimer <= 0f)
            {
                isWaypointPausing = false;
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
            return;
        }

        float toTargetX = waypoints[currentWaypointIndex].position.x - transform.position.x;
        MoveOnX(toTargetX, data.patrolSpeed, 0.2f, () => StartWaypointPause());
    }

    private void UpdateAlert()
    {
        SetVelocityX(0f);
        alertTimer -= Time.deltaTime;
        if (alertTimer <= 0f) ChangeState(EnemyState.Chase);
    }

    private void UpdateChase()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > data.stopChaseRange)
        {
            ChangeState(EnemyState.Return);
            return;
        }

        float toPlayerX = player.position.x - transform.position.x;
        MoveOnX(toPlayerX, data.chaseSpeed, 0.5f, null);
    }

    private void UpdateReturn()
    {
        float targetX = (waypoints != null && waypoints.Length > 0)
                    ? waypoints[0].position.x
                    : startPosition.x;

        float toTargetX = targetX - transform.position.x;
        MoveOnX(toTargetX, data.patrolSpeed, 0.3f, () => ChangeState(EnemyState.Patrol));
    }

    private void FindPlayerReference()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return;
        player = playerObj.transform;
        playerStealth = playerObj.GetComponent<PlayerStealth>();
    }

    private bool CanSeePlayer()
    {
        if (player == null) return false;

        float range = data.sightRange;
        if (playerStealth != null && playerStealth.IsStealthing)
            range *= data.stealthDetectionMultiplier;

        if (Vector2.Distance(transform.position, player.position) > range)
            return false;

        Vector2 direction = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, range, detectionLayers);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            Debug.DrawRay(transform.position, direction * hit.distance, Color.red);
            return true;
        }

        return false;
    }

    public void ChangeState(EnemyState newState)
    {
        currentState = newState;

        if (newState == EnemyState.Alert)
            alertTimer = data != null ? data.alertDuration : 0.8f;

        UpdateAlertBubble();
    }

    public void Alert() => ChangeState(EnemyState.Alert);

    private void FlipSprite(float directionX)
    {
        if (directionX > 0.1f && !isFacingRight)
        {
            isFacingRight = true;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        else if (directionX < -0.1f && isFacingRight)
        {
            isFacingRight = false;
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    private void MoveOnX(float deltaX, float speed, float threshold, System.Action onArrived)
    {
        if (Mathf.Abs(deltaX) < threshold)
        {
            SetVelocityX(0f);
            onArrived?.Invoke();
        }
        else
        {
            float dirX = Mathf.Sign(deltaX);
            SetVelocityX(dirX * speed);
            FlipSprite(dirX);
        }
    }

    private void SetVelocityX(float x)
    {
        rb.linearVelocity = new Vector2(x, rb.linearVelocity.y);
    }

    private void StartWaypointPause()
    {
        isWaypointPausing = true;
        waypointPauseTimer = data != null ? data.waypointPauseTime : 0.5f;
    }

    private void UpdateAlertBubble()
    {
        if (alertBubble == null) return;
        switch (currentState)
        {
            case EnemyState.Alert:
                alertBubble.color = Color.yellow;
                alertBubble.gameObject.SetActive(true);
                break;
            case EnemyState.Chase:
                alertBubble.color = Color.red;
                alertBubble.gameObject.SetActive(true);
                break;
            default:
                alertBubble.gameObject.SetActive(false);
                break;
        }
    }

}

