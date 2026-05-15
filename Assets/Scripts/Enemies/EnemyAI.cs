using UnityEngine;

public enum EnemyState { Idle, Patrol, Alert, Chase, Return }

public class EnemyAI : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private EnemySO data;

    [Header("State")]
    [SerializeField] private EnemyState currentState = EnemyState.Patrol;

    [Header("Patrol Points")]
    [SerializeField] private Transform[] waypoints;

    [Header("References")]
    [SerializeField] private SpriteRenderer alertBubble;
    [SerializeField] private LayerMask detectionLayers;

    private EnemyFlying flying;
    private EnemyShooting shooting;

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
        if (data == null)
            Debug.LogError($"EnemyAI on {gameObject.name} is missing its EnemySO reference!");

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError($"EnemyAI on {gameObject.name} requires a Rigidbody2D component.");

        flying = GetComponent<EnemyFlying>();
        shooting = GetComponent<EnemyShooting>();

        if (flying != null)
            flying.Init(rb, data);

        if (shooting != null)
            shooting.Init(data);

        PlayerDamageDealer dealer = GetComponent<PlayerDamageDealer>();
        if (dealer != null)
            dealer.Initialize(data.contactDamage, data.damageInterval);
    }

    private void Start()
    {
        FindPlayerReference();
        startPosition = transform.position;
        if (alertBubble != null) alertBubble.gameObject.SetActive(false);
        ChangeState(currentState);
    }

    private void Update()
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

    // -------------------------------------------------------------------------
    // States
    // -------------------------------------------------------------------------

    private void UpdateIdle()
    {
        SetVelocityX(0f);
        if (CanSeePlayer()) ChangeState(EnemyState.Alert);
    }

    private void UpdatePatrol()
    {
        if (CanSeePlayer()) { ChangeState(EnemyState.Alert); return; }

        if (waypoints == null || waypoints.Length == 0) { SetVelocityX(0f); return; }

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

        MoveToPoint(waypoints[currentWaypointIndex].position, data.patrolSpeed, 0.2f, StartWaypointPause);
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

        if (IsOutsideStopChaseRange())
        {
            ChangeState(EnemyState.Return);
            return;
        }

        if (IsOutsideLeashBoundary())
        {
            HoldAtBoundary();
            return;
        }

        if (shooting != null)
            shooting.UpdateCombat(player, rb, MoveToward);
        else
            MoveToward(player.position);
    }

    private void UpdateReturn()
    {
        if (CanSeePlayer()) { ChangeState(EnemyState.Alert); return; }

        Vector2 targetPos = startPosition;
        int closestIndex = 0;

        if (waypoints != null && waypoints.Length > 0)
        {
            float minDist = float.MaxValue;
            for (int i = 0; i < waypoints.Length; i++)
            {
                float dist = Vector2.Distance(transform.position, waypoints[i].position);
                if (dist < minDist) { minDist = dist; targetPos = waypoints[i].position; closestIndex = i; }
            }
        }

        MoveToPoint(targetPos, data.patrolSpeed, 0.3f, () =>
        {
            currentWaypointIndex = closestIndex;
            ChangeState(waypoints != null && waypoints.Length > 0 ? EnemyState.Patrol : EnemyState.Idle);
        });
    }

    // -------------------------------------------------------------------------
    // Chase helpers
    // -------------------------------------------------------------------------

    private bool IsOutsideStopChaseRange()
    {
        if (flying != null)
            return Vector2.Distance(transform.position, player.position) > data.stopChaseRange;

        float dx = Mathf.Abs(player.position.x - transform.position.x);
        float dy = Mathf.Abs(player.position.y - transform.position.y);
        return dx > data.stopChaseBoxSize.x / 2f || dy > data.stopChaseBoxSize.y / 2f;
    }

    private bool IsOutsideLeashBoundary()
    {
        if (flying != null)
            return flying.IsOutsideBoundary(transform.position, player.position);

        float dx = Mathf.Abs(transform.position.x - startPosition.x);
        float dy = Mathf.Abs(transform.position.y - startPosition.y);
        float pdx = Mathf.Abs(player.position.x - startPosition.x);
        float pdy = Mathf.Abs(player.position.y - startPosition.y);
        float hw = data.maxChaseBoxSize.x / 2f;
        float hh = data.maxChaseBoxSize.y / 2f;
        return (dx >= hw || dy >= hh) && (pdx > hw || pdy > hh);
    }

    private void HoldAtBoundary()
    {
        rb.linearVelocity = flying != null ? Vector2.zero : new Vector2(0f, rb.linearVelocity.y);
        FlipSprite(player.position.x - transform.position.x);

        if (shooting != null)
            shooting.TryShootIfInRange(player, rb);
    }

    private void MoveToward(Vector2 target)
    {
        MoveToPoint(target, data.chaseSpeed, 0.5f, null);
    }

    // -------------------------------------------------------------------------
    // Movement
    // -------------------------------------------------------------------------

    private void MoveToPoint(Vector2 target, float speed, float threshold, System.Action onArrived)
    {
        if (Vector2.Distance(transform.position, target) < threshold)
        {
            rb.linearVelocity = flying != null ? Vector2.zero : new Vector2(0f, rb.linearVelocity.y);
            onArrived?.Invoke();
            return;
        }

        Vector2 direction = (target - (Vector2)transform.position).normalized;

        if (flying != null)
            rb.linearVelocity = direction * speed;
        else
            SetVelocityX(direction.x * speed);

        FlipSprite(direction.x);
    }

    private void SetVelocityX(float x) =>
        rb.linearVelocity = new Vector2(x, rb.linearVelocity.y);

    private void FlipSprite(float directionX)
    {
        if (directionX > 0.1f && !isFacingRight)
        {
            isFacingRight = true;
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x);
            transform.localScale = s;
        }
        else if (directionX < -0.1f && isFacingRight)
        {
            isFacingRight = false;
            Vector3 s = transform.localScale;
            s.x = -Mathf.Abs(s.x);
            transform.localScale = s;
        }
    }

    // -------------------------------------------------------------------------
    // Detection
    // -------------------------------------------------------------------------

    private bool CanSeePlayer()
    {
        if (data.patrolOnly || player == null) return false;

        bool inZone;

        if (flying != null)
        {
            float range = data.sightRange;
            if (playerStealth != null)
            {
                if (playerStealth.IsStealthing) range *= data.stealthDetectionMultiplier;
                if (playerStealth.IsInDarkZone) range *= data.darkZoneDetectionMultiplier;
            }
            inZone = Vector2.Distance(transform.position, player.position) <= range;
        }
        else
        {
            Vector2 box = data.sightBoxSize;
            if (playerStealth != null)
            {
                if (playerStealth.IsStealthing) box *= data.stealthDetectionMultiplier;
                if (playerStealth.IsInDarkZone) box *= data.darkZoneDetectionMultiplier;
            }
            float dx = Mathf.Abs(player.position.x - transform.position.x);
            float dy = Mathf.Abs(player.position.y - transform.position.y);
            inZone = dx <= box.x / 2f && dy <= box.y / 2f;
        }

        if (!inZone) return false;

        Vector2 dir = (player.position - transform.position).normalized;
        float dist = Vector2.Distance(transform.position, player.position);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dist, detectionLayers);
        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    // -------------------------------------------------------------------------
    // State management
    // -------------------------------------------------------------------------

    public void ChangeState(EnemyState newState)
    {
        currentState = newState;
        if (newState == EnemyState.Alert)
            alertTimer = data != null ? data.alertDuration : 0.8f;
        UpdateAlertBubble();
    }

    public void Alert() => ChangeState(EnemyState.Alert);

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

    // -------------------------------------------------------------------------
    // Misc
    // -------------------------------------------------------------------------

    private void FindPlayerReference()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return;
        player = playerObj.transform;
        playerStealth = playerObj.GetComponent<PlayerStealth>();
    }

    private void StartWaypointPause()
    {
        isWaypointPausing = true;
        waypointPauseTimer = data != null ? data.waypointPauseTime : 0.5f;
    }

    // -------------------------------------------------------------------------
    // Gizmos
    // -------------------------------------------------------------------------

    private void OnDrawGizmosSelected()
    {
        if (data == null) return;

        bool isFlying = flying != null || GetComponent<EnemyFlying>() != null;

        Gizmos.color = Color.yellow;
        if (isFlying)
            Gizmos.DrawWireSphere(transform.position, data.sightRange);
        else
            Gizmos.DrawWireCube(transform.position, new Vector3(data.sightBoxSize.x, data.sightBoxSize.y, 0f));

        Gizmos.color = Color.gray;
        if (isFlying)
            Gizmos.DrawWireSphere(transform.position, data.stopChaseRange);
        else
            Gizmos.DrawWireCube(transform.position, new Vector3(data.stopChaseBoxSize.x, data.stopChaseBoxSize.y, 0f));

        Gizmos.color = Color.cyan;
        Vector2 center = Application.isPlaying ? startPosition : (Vector2)transform.position;
        if (isFlying)
            Gizmos.DrawWireSphere(center, data.maxChaseDistance);
        else
            Gizmos.DrawWireCube(center, new Vector3(data.maxChaseBoxSize.x, data.maxChaseBoxSize.y, 0f));
    }
}