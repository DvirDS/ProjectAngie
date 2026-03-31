using UnityEngine;

public enum EnemyState { Idle, Patrol, Alert, Chase, Return }

public class EnemyAI : MonoBehaviour
{
    [Header("State Settings")]
    public EnemyState currentState = EnemyState.Patrol;

    [Header("Movement & Detection")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;
    public float sightRange = 8f;
    [SerializeField] private float stealthDetectionMultiplier = 0.3f;
    public float stopChaseRange = 12f;
    public LayerMask detectionLayers;

    [Header("Patrol Points (Optional)")]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    [Header("References")]
    public SpriteRenderer alertBubble;

    private Transform player;
    private PlayerStealth playerStealth; 
    private Rigidbody2D rb;
    private bool isFacingRight = true;
    private Vector2 startPosition;

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

    private void FindPlayerReference()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerStealth = playerObj.GetComponent<PlayerStealth>();
        }
    }

    private bool CanSeePlayer()
    {
        if (player == null) return false;

        float currentSightRange = sightRange;

        if (playerStealth != null && playerStealth.IsStealthing)
        {
            currentSightRange *= stealthDetectionMultiplier;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > currentSightRange)
        {
            return false;
        }

        Vector2 direction = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, currentSightRange, detectionLayers);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.DrawRay(transform.position, direction * hit.distance, Color.red);
                return true;
            }
        }

        return false;
    }

    private void UpdateIdle()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
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
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        float directionX = Mathf.Sign(targetWaypoint.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(directionX * patrolSpeed, rb.linearVelocity.y);
        FlipSprite(directionX);

        if (Mathf.Abs(transform.position.x - targetWaypoint.position.x) < 0.5f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length) currentWaypointIndex = 0;
        }
    }

    private void UpdateAlert()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        ChangeState(EnemyState.Chase);
    }

    private void UpdateChase()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > stopChaseRange)
        {
            ChangeState(EnemyState.Return);
            return;
        }

        float distanceX = Mathf.Abs(player.position.x - transform.position.x);

        if (distanceX < 0.5f)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else
        {
            float directionX = Mathf.Sign(player.position.x - transform.position.x);
            rb.linearVelocity = new Vector2(directionX * chaseSpeed, rb.linearVelocity.y);
            FlipSprite(directionX);
        }
    }

    private void UpdateReturn()
    {
        Vector2 targetPos = (waypoints != null && waypoints.Length > 0) ? (Vector2)waypoints[0].position : startPosition;

        float directionX = Mathf.Sign(targetPos.x - transform.position.x);
        rb.linearVelocity = new Vector2(directionX * patrolSpeed, rb.linearVelocity.y);
        FlipSprite(directionX);

        if (Mathf.Abs(transform.position.x - targetPos.x) < 0.5f)
        {
            ChangeState(EnemyState.Patrol);
        }
    }

    public void ChangeState(EnemyState newState)
    {
        currentState = newState;

        if (alertBubble != null)
        {
            if (currentState == EnemyState.Alert)
            {
                alertBubble.color = Color.yellow;
                alertBubble.gameObject.SetActive(true);
            }
            else if (currentState == EnemyState.Chase)
            {
                alertBubble.color = Color.red;
                alertBubble.gameObject.SetActive(true);
            }
            else
            {
                alertBubble.gameObject.SetActive(false);
            }
        }
    }

    private void FlipSprite(float directionX)
    {
        if (directionX > 0 && !isFacingRight)
        {
            isFacingRight = true;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (directionX < -0.1f && isFacingRight)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}