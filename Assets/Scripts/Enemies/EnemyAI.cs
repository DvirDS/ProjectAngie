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

    [Header("Combat Settings")]
    [SerializeField] private Transform firePoint;
    private float fireTimer = 0f;

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
    private float lastDamageTime;

    private void Awake()
    {
        // Safety check for the data file reference
        if (data == null)
        {
            Debug.LogError($"EnemyAI on {gameObject.name} is missing the EnemySO (Data) reference!");
        }

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError($"EnemyAI on {gameObject.name} requires a Rigidbody2D component.");
        }
        else if (canFly)
        {
            rb.gravityScale = 0f;
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

        MoveToPoint(waypoints[currentWaypointIndex].position, data.patrolSpeed, 0.2f, () => StartWaypointPause());
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

    // --- הנה השורה שהייתה חסרה! ---
    float distanceToPlayer = Vector2.Distance(transform.position, player.position);

    bool lostPlayer = false;

    // 1. Did the player escape the stop chase boundary?
    if (canFly)
    {
        // עכשיו אפשר להשתמש במשתנה שחישבנו
        lostPlayer = distanceToPlayer > data.stopChaseRange;
    }
    else
    {
        float chaseDiffX = Mathf.Abs(player.position.x - transform.position.x);
        float chaseDiffY = Mathf.Abs(player.position.y - transform.position.y);
        lostPlayer = (chaseDiffX > data.stopChaseBoxSize.x / 2f || chaseDiffY > data.stopChaseBoxSize.y / 2f);
    }

    // If yes, give up and return
    if (lostPlayer)
    {
        ChangeState(EnemyState.Return);
        return;
    }

    // --- 2. The Smart Leash Mechanism! ---
    bool isOutsideBoundary = false;

    if (canFly)
    {
        // Circle logic for flying enemies
        float distanceFromStart = Vector2.Distance(startPosition, transform.position);
        float playerDistanceFromStart = Vector2.Distance(startPosition, player.position);
        
        isOutsideBoundary = (distanceFromStart >= data.maxChaseDistance && playerDistanceFromStart > data.maxChaseDistance);
    }
    else
    {
        // Box logic for ground enemies
        float diffX = Mathf.Abs(transform.position.x - startPosition.x);
        float diffY = Mathf.Abs(transform.position.y - startPosition.y);
        
        float playerDiffX = Mathf.Abs(player.position.x - startPosition.x);
        float playerDiffY = Mathf.Abs(player.position.y - startPosition.y);

        float halfWidth = data.maxChaseBoxSize.x / 2f;
        float halfHeight = data.maxChaseBoxSize.y / 2f;

        isOutsideBoundary = (diffX >= halfWidth || diffY >= halfHeight) && 
                            (playerDiffX > halfWidth || playerDiffY > halfHeight);
    }

    // If the enemy reached the boundary edge AND the player is outside this boundary
    if (isOutsideBoundary)
    {
        // Full stop! This prevents the jittering
        rb.linearVelocity = canFly ? Vector2.zero : new Vector2(0f, rb.linearVelocity.y);

        // Ensure the enemy still faces the player instead of freezing
        float directionX = player.position.x - transform.position.x;
        FlipSprite(directionX);

        // Bonus: If it's a shooting enemy, it stays at the boundary and shoots if in range!
        if (canFly && data.projectilePrefab != null && distanceToPlayer <= data.shootingRange)
        {
            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                Shoot();
                fireTimer = data.fireRate;
            }
        }
        
        return; // Stop the function here so the enemy doesn't try to move further
    }

    // --- 3. Normal chase logic (if we are within the boundary) ---
    if (canFly && data.projectilePrefab != null)
    {
        if (distanceToPlayer <= data.shootingRange)
        {
            // Within shooting range: stop moving to aim and shoot
            SetVelocityX(0f);
            rb.linearVelocity = Vector2.zero;
            
            // Ensure the enemy faces the player
            float directionX = player.position.x - transform.position.x;
            FlipSprite(directionX);

            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                Shoot();
                fireTimer = data.fireRate; // Reset timer for the next shot
            }
        }
        else
        {
            // Too far to shoot: move closer to the player
            MoveToPoint(player.position, data.chaseSpeed, 0.5f, null);
        }
    }
    else
    {
        // Standard melee enemy logic: move towards the player
        MoveToPoint(player.position, data.chaseSpeed, 0.5f, null);
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
        // Keep eyes open while returning
        if (CanSeePlayer())
        {
            ChangeState(EnemyState.Alert);
            return;
        }

        Vector2 targetPos = startPosition;
        int closestIndex = 0;

        // Logic to find the closest waypoint
        if (waypoints != null && waypoints.Length > 0)
        {
            float minDistance = float.MaxValue;

            for (int i = 0; i < waypoints.Length; i++)
            {
                float dist = Vector2.Distance(transform.position, waypoints[i].position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    targetPos = waypoints[i].position;
                    closestIndex = i; // Save the index to continue patrol from here
                }
            }
        }

        // Move to the closest point found
        MoveToPoint(targetPos, data.patrolSpeed, 0.3f, () =>
        {
            currentWaypointIndex = closestIndex; // Set the next patrol point

            if (waypoints != null && waypoints.Length > 0)
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
        if (player == null) return false;

        bool isInsideZone = false;

        if (canFly)
        {
            // --- Circle Logic (Flying) ---
            float currentRange = data.sightRange;
            if (playerStealth != null)
            {
                if (playerStealth.IsStealthing) currentRange *= data.stealthDetectionMultiplier;
                if (playerStealth.IsInDarkZone) currentRange *= data.darkZoneDetectionMultiplier;
            }
            isInsideZone = Vector2.Distance(transform.position, player.position) <= currentRange;
        }
        else
        {
            // --- Box Logic (Ground) ---
            Vector2 currentBox = data.sightBoxSize;
            if (playerStealth != null)
            {
                if (playerStealth.IsStealthing) currentBox *= data.stealthDetectionMultiplier;
                if (playerStealth.IsInDarkZone) currentBox *= data.darkZoneDetectionMultiplier;
            }

            float diffX = Mathf.Abs(player.position.x - transform.position.x);
            float diffY = Mathf.Abs(player.position.y - transform.position.y);

            // Check if inside width and height
            isInsideZone = (diffX <= currentBox.x / 2f && diffY <= currentBox.y / 2f);
        }

        // If Angie is mathematically inside the zone, shoot a raycast to make sure there are no walls
        if (isInsideZone)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            float dist = Vector2.Distance(transform.position, player.position);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, dist, detectionLayers);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                return true;
            }
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

    private void MoveToPoint(Vector2 targetPos, float speed, float threshold, System.Action onArrived)
    {
        float distance = Vector2.Distance(transform.position, targetPos);

        // Check if we reached the target
        if (distance < threshold)
        {
            // Stop moving: Flying enemies stop completely, ground enemies stop X movement but keep Y (gravity)
            rb.linearVelocity = canFly ? Vector2.zero : new Vector2(0f, rb.linearVelocity.y);
            onArrived?.Invoke();
        }
        else
        {
            // Calculate the direction towards the target
            Vector2 direction = (targetPos - (Vector2)transform.position).normalized;

            if (canFly)
            {
                // Free movement towards the target (Air)
                rb.linearVelocity = direction * speed;
            }
            else
            {
                // Movement only on the X axis (Ground)
                SetVelocityX(direction.x * speed);
            }

            // Face the correct direction
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
    private void OnTriggerStay2D(Collider2D other)
    {
        if (canFly) return;

        if (other.CompareTag("Player"))
        {
            if (Time.time >= lastDamageTime + data.damageInterval)
            {
                HealthDrainSystem playerHealth = other.GetComponent<HealthDrainSystem>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(data.contactDamage);
                    lastDamageTime = Time.time;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (data == null) return;

        if (canFly)
        {
            // --- Flying Enemies (Circles) ---
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, data.sightRange);

            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, data.stopChaseRange);
        }
        else
        {
            // --- Ground Enemies (Boxes) ---
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, new Vector3(data.sightBoxSize.x, data.sightBoxSize.y, 0f));

            Gizmos.color = Color.gray;
            Gizmos.DrawWireCube(transform.position, new Vector3(data.stopChaseBoxSize.x, data.stopChaseBoxSize.y, 0f));
        }

        // Leash Boundary - Blue
        Gizmos.color = Color.cyan;
        Vector2 centerPoint = Application.isPlaying ? startPosition : (Vector2)transform.position;

        if (canFly)
            Gizmos.DrawWireSphere(centerPoint, data.maxChaseDistance);
        else
            Gizmos.DrawWireCube(centerPoint, new Vector3(data.maxChaseBoxSize.x, data.maxChaseBoxSize.y, 0f));
    }

}

