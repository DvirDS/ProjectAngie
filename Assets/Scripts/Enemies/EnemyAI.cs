using UnityEngine;

public enum EnemyState { Idle, Patrol, Alert, Chase, Return }

public class EnemyAI : MonoBehaviour
{
    private const float ZeroVelocity = 0f;
    private const float ZeroGravity = 0f;
    private const float InitialTimer = 0f;
    private const int EmptyWaypointsLength = 0;
    private const int FirstWaypointIndex = 0;
    private const int NextWaypointIncrement = 1;
    private const float HalfDimensionDivider = 2f;
    private const float PatrolArrivalThreshold = 0.2f;
    private const float ChaseArrivalThreshold = 0.5f;
    private const float ReturnArrivalThreshold = 0.3f;
    private const float FlipThreshold = 0.1f;
    private const float FallbackAlertDuration = 0.8f;
    private const float FallbackWaypointPauseTime = 0.5f;
    private const float GizmoZeroZ = 0f;

    [Header("Data")]
    [SerializeField] private EnemySO data;

    [Header("State Settings")]
    public EnemyState currentState = EnemyState.Patrol;

    [Header("Patrol Points")]
    public Transform[] waypoints;

    [Header("Combat Settings")]
    [SerializeField] private Transform firePoint;
    private float fireTimer = InitialTimer;

    [Header("References")]
    [SerializeField] private SpriteRenderer alertBubble;
    [SerializeField] private LayerMask detectionLayers;

    private Rigidbody2D rb;
    private Transform player;
    private PlayerStealth playerStealth;
    private bool isFacingRight = true;
    private Vector2 startPosition;
    private int currentWaypointIndex = FirstWaypointIndex;

    private float alertTimer = InitialTimer;
    private float waypointPauseTimer = InitialTimer;
    private bool isWaypointPausing = false;
    private float lastDamageTime;

    private void OnEnable()
    {
        LightBeamDetector.OnPlayerCaughtInBeam += Alert;
    }

    private void OnDisable()
    {
        LightBeamDetector.OnPlayerCaughtInBeam -= Alert;
    }

    private void Awake()
    {
        if (data == null)
        {
            Debug.LogError($"EnemyAI on {gameObject.name} is missing the EnemySO (Data) reference!");
        }

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError($"EnemyAI on {gameObject.name} requires a Rigidbody2D component.");
        }
        else if (data.canFly)
        {
            rb.gravityScale = ZeroGravity;
        }
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

    public void Freeze() => ChangeState(EnemyState.Idle);
    public void Unfreeze() => ChangeState(EnemyState.Patrol);

    private void UpdateIdle()
    {
        SetVelocityX(ZeroVelocity);
        if (CanSeePlayer()) ChangeState(EnemyState.Alert);
    }

    private void UpdatePatrol()
    {
        if (CanSeePlayer())
        {
            ChangeState(EnemyState.Alert);
            return;
        }

        if (waypoints == null || waypoints.Length == EmptyWaypointsLength)
        {
            SetVelocityX(ZeroVelocity);
            return;
        }

        if (isWaypointPausing)
        {
            SetVelocityX(ZeroVelocity);
            waypointPauseTimer -= Time.deltaTime;
            if (waypointPauseTimer <= ZeroVelocity)
            {
                isWaypointPausing = false;
                currentWaypointIndex = (currentWaypointIndex + NextWaypointIncrement) % waypoints.Length;
            }
            return;
        }

        MoveToPoint(waypoints[currentWaypointIndex].position, data.patrolSpeed, PatrolArrivalThreshold, () => StartWaypointPause());
    }

    private void UpdateAlert()
    {
        SetVelocityX(ZeroVelocity);
        alertTimer -= Time.deltaTime;
        if (alertTimer <= ZeroVelocity)
        {
            bool staysHidden = data.patrolOnly || (playerStealth != null && playerStealth.IsInDarkZone);
            ChangeState(staysHidden ? EnemyState.Patrol : EnemyState.Chase);
        }
    }

    private void UpdateChase()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool lostPlayer = false;

        if (data.canFly)
        {
            lostPlayer = distanceToPlayer > data.stopChaseRange;
        }
        else
        {
            float chaseDiffX = Mathf.Abs(player.position.x - transform.position.x);
            float chaseDiffY = Mathf.Abs(player.position.y - transform.position.y);
            lostPlayer = (chaseDiffX > data.stopChaseBoxSize.x / HalfDimensionDivider || chaseDiffY > data.stopChaseBoxSize.y / HalfDimensionDivider);
        }

        if (!lostPlayer && data.canFly)
        {
            if (!CanSeePlayer())
            {
                lostPlayer = true;
            }
        }

        if (lostPlayer)
        {
            ChangeState(EnemyState.Return);
            return;
        }

        bool isOutsideBoundary = false;

        if (data.canFly)
        {
            float distanceFromStart = Vector2.Distance(startPosition, transform.position);
            float playerDistanceFromStart = Vector2.Distance(startPosition, player.position);

            isOutsideBoundary = (distanceFromStart >= data.maxChaseDistance && playerDistanceFromStart > data.maxChaseDistance);
        }
        else
        {
            float diffX = Mathf.Abs(transform.position.x - startPosition.x);
            float diffY = Mathf.Abs(transform.position.y - startPosition.y);

            float playerDiffX = Mathf.Abs(player.position.x - startPosition.x);
            float playerDiffY = Mathf.Abs(player.position.y - startPosition.y);

            float halfWidth = data.maxChaseBoxSize.x / HalfDimensionDivider;
            float halfHeight = data.maxChaseBoxSize.y / HalfDimensionDivider;

            isOutsideBoundary = (diffX >= halfWidth || diffY >= halfHeight) &&
                                (playerDiffX > halfWidth || playerDiffY > halfHeight);
        }

        if (isOutsideBoundary)
        {
            rb.linearVelocity = data.canFly ? Vector2.zero : new Vector2(ZeroVelocity, rb.linearVelocity.y);

            float directionX = player.position.x - transform.position.x;
            FlipSprite(directionX);

            if (data.canFly && data.projectilePrefab != null && distanceToPlayer <= data.shootingRange)
            {
                fireTimer -= Time.deltaTime;
                if (fireTimer <= ZeroVelocity)
                {
                    Shoot();
                    fireTimer = data.fireRate;
                }
            }

            return;
        }

        if (data.canFly && data.projectilePrefab != null)
        {
            if (distanceToPlayer <= data.shootingRange)
            {
                SetVelocityX(ZeroVelocity);
                rb.linearVelocity = Vector2.zero;

                float directionX = player.position.x - transform.position.x;
                FlipSprite(directionX);

                fireTimer -= Time.deltaTime;
                if (fireTimer <= ZeroVelocity)
                {
                    Shoot();
                    fireTimer = data.fireRate;
                }
            }
            else
            {
                MoveToPoint(player.position, data.chaseSpeed, ChaseArrivalThreshold, null);
            }
        }
        else
        {
            MoveToPoint(player.position, data.chaseSpeed, ChaseArrivalThreshold, null);
        }
    }

    private void Shoot()
    {
        if (data.projectilePrefab == null) return;

        Vector2 spawnPos = firePoint != null ? (Vector2)firePoint.position : (Vector2)transform.position;
        GameObject bullet = Instantiate(data.projectilePrefab, spawnPos, Quaternion.identity);

        Vector2 shootDirection = (player.position - (Vector3)spawnPos).normalized;

        Projectile bulletScript = bullet.GetComponent<Projectile>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(shootDirection, data.projectileSpeed);
        }
    }

    private void UpdateReturn()
    {
        if (CanSeePlayer())
        {
            ChangeState(EnemyState.Alert);
            return;
        }

        Vector2 targetPos = startPosition;
        int closestIndex = FirstWaypointIndex;

        if (waypoints != null && waypoints.Length > EmptyWaypointsLength)
        {
            float minDistance = float.MaxValue;

            for (int i = FirstWaypointIndex; i < waypoints.Length; i++)
            {
                float dist = Vector2.Distance(transform.position, waypoints[i].position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    targetPos = waypoints[i].position;
                    closestIndex = i;
                }
            }
        }

        MoveToPoint(targetPos, data.patrolSpeed, ReturnArrivalThreshold, () =>
        {
            currentWaypointIndex = closestIndex;

            if (waypoints != null && waypoints.Length > EmptyWaypointsLength)
                ChangeState(EnemyState.Patrol);
            else
                ChangeState(EnemyState.Idle);
        });
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
        if (data.patrolOnly) return false;
        if (player == null) return false;
        if (playerStealth != null && playerStealth.IsInDarkZone) return false;

        bool isInsideZone = false;

        if (data.canFly)
        {
            float currentRange = data.sightRange;
            if (playerStealth != null && playerStealth.IsStealthing) currentRange *= data.stealthDetectionMultiplier;
            isInsideZone = Vector2.Distance(transform.position, player.position) <= currentRange;
        }
        else
        {
            Vector2 currentBox = data.sightBoxSize;
            if (playerStealth != null && playerStealth.IsStealthing) currentBox *= data.stealthDetectionMultiplier;

            float diffX = Mathf.Abs(player.position.x - transform.position.x);
            float diffY = Mathf.Abs(player.position.y - transform.position.y);

            isInsideZone = (diffX <= currentBox.x / HalfDimensionDivider && diffY <= currentBox.y / HalfDimensionDivider);
        }

        if (isInsideZone)
        {
            if (data.canFly)
            {
                RaycastHit2D hit = Physics2D.Linecast(transform.position, player.position, detectionLayers);

                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        return true;
                    }

                    return false;
                }
            }
            else
            {
                Vector2 direction = (player.position - transform.position).normalized;
                float dist = Vector2.Distance(transform.position, player.position);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, dist, detectionLayers);

                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void ChangeState(EnemyState newState)
    {
        currentState = newState;

        if (newState == EnemyState.Alert)
            alertTimer = data != null ? data.alertDuration : FallbackAlertDuration;

        UpdateAlertBubble();
    }

    public void Alert() => ChangeState(EnemyState.Alert);

    private void FlipSprite(float directionX)
    {
        if (directionX > FlipThreshold && !isFacingRight)
        {
            isFacingRight = true;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        else if (directionX < -FlipThreshold && isFacingRight)
        {
            isFacingRight = false;
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    private void MoveToPoint(Vector2 targetPos, float speed, float threshold, System.Action onArrived)
    {
        float distance = data.canFly
            ? Vector2.Distance(transform.position, targetPos)
            : Mathf.Abs(transform.position.x - targetPos.x);

        if (distance < threshold)
        {
            rb.linearVelocity = data.canFly ? Vector2.zero : new Vector2(ZeroVelocity, rb.linearVelocity.y);
            onArrived?.Invoke();
        }
        else
        {
            Vector2 direction = (targetPos - (Vector2)transform.position).normalized;

            if (data.canFly)
            {
                rb.linearVelocity = direction * speed;
            }
            else
            {
                SetVelocityX(direction.x * speed);
            }

            FlipSprite(direction.x);
        }
    }

    private void SetVelocityX(float x)
    {
        rb.linearVelocity = new Vector2(x, rb.linearVelocity.y);
    }

    private void StartWaypointPause()
    {
        isWaypointPausing = true;
        waypointPauseTimer = data != null ? data.waypointPauseTime : FallbackWaypointPauseTime;
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

    private void OnDrawGizmosSelected()
    {
        if (data == null) return;

        if (data.canFly)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, data.sightRange);

            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, data.stopChaseRange);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, new Vector3(data.sightBoxSize.x, data.sightBoxSize.y, GizmoZeroZ));

            Gizmos.color = Color.gray;
            Gizmos.DrawWireCube(transform.position, new Vector3(data.stopChaseBoxSize.x, data.stopChaseBoxSize.y, GizmoZeroZ));
        }

        Gizmos.color = Color.cyan;
        Vector2 centerPoint = Application.isPlaying ? startPosition : (Vector2)transform.position;

        if (data.canFly)
            Gizmos.DrawWireSphere(centerPoint, data.maxChaseDistance);
        else
            Gizmos.DrawWireCube(centerPoint, new Vector3(data.maxChaseBoxSize.x, data.maxChaseBoxSize.y, GizmoZeroZ));
    }
}