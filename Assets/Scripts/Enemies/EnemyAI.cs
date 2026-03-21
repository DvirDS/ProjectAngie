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
    public float stopChaseRange = 12f;
    public LayerMask detectionLayers; // שכבות שהראייה נחסמת בהן (חובה לבחור את שכבת השחקן ב-Inspector)

    [Header("Patrol Points (Optional)")]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    [Header("References")]
    public SpriteRenderer alertBubble;

    // משתנים נסתרים שמנוהלים אוטומטית
    private Transform player;
    private Rigidbody2D rb;
    private bool isFacingRight = true;
    private Vector2 startPosition; // שומר את מיקום ההתחלה למקרה שאין Waypoints

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
        // --- פתרון חסין לטעינת סצנות (Multi-Scene Failsafe) ---
        // אם המשחק התחיל ועדיין לא מצאנו את הכלבה (כי הסצנה שלה נטענה שנייה אחרי), נמשיך לחפש
        if (player == null)
        {
            FindPlayerReference();
            if (player == null) return; // אל תעשה כלום עד שהכלבה תופיע במשחק
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
        // מחפש אובייקט עם התג Player בעולם
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    // --- מערכת הראייה עם צבעים לדיבאג ---
    private bool CanSeePlayer()
    {
        // סיבה 1: האם הכלבה חסרה?
        if (player == null)
        {
            return false;
        }

        // סיבה 2: האם המרחק גדול מדי?
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > sightRange)
        {
            return false;
        }

        // --- כאן אנחנו בודקים מה חוסם את הקרן ---
        Vector2 direction = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, sightRange, detectionLayers);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("הללויה! הקרן פגעה בכלבה!");
                Debug.DrawRay(transform.position, direction * hit.distance, Color.red);
                return true;
            }
            else
            {
                // פה נגלה מי מסתיר לאויב את הראייה!
                Debug.Log("הקרן נחסמה! היא פגעה באובייקט בשם: " + hit.collider.gameObject.name);
                Debug.DrawRay(transform.position, direction * hit.distance, Color.white);
            }
        }
        else
        {
            Debug.Log("הקרן לא פגעה בכלום! (עברה דרך הכלבה, אולי חסר לה קוליידר?)");
            Debug.DrawRay(transform.position, direction * sightRange, Color.green);
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
        // 1. קודם כל ולפני הכל - בודקים אם רואים את הכלבה!
        if (CanSeePlayer())
        {
            ChangeState(EnemyState.Alert);
            return;
        }

        // 2. אם אין נקודות ציון (Waypoints), פשוט עומדים במקום. הראייה עדיין תעבוד כי היא בשלב 1.
        if (waypoints == null || waypoints.Length == 0)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // 3. תנועת סיור ימינה/שמאלה
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
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // עוצר במקום כדי לחשד
        // כרגע עובר מיד למרדף. בהמשך אפשר לשים פה טיימר.
        ChangeState(EnemyState.Chase);
    }

    private void UpdateChase()
    {
        if (player == null) return;

        // קודם כל, בודקים אם היא ברחה לנו מחוץ לטווח המרדף
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > stopChaseRange)
        {
            ChangeState(EnemyState.Return);
            return;
        }

        // --- הפתרון לרטט: עוצרים אם אנחנו ממש קרובים! ---
        // אנחנו בודקים רק את ציר ה-X כדי שלא ירעד
        float distanceX = Mathf.Abs(player.position.x - transform.position.x);

        if (distanceX < 0.5f) // ברגע שהוא במרחק של חצי יחידה מהכלבה - הוא עוצר
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // עצור במקום

            // --- כאן בדיוק נכניס בהמשך את הקוד של ה-Game Over / פגיעה בשחקן! ---
        }
        else
        {
            // אם אנחנו עדיין רחוקים, ממשיכים לרדוף
            float directionX = Mathf.Sign(player.position.x - transform.position.x);
            rb.linearVelocity = new Vector2(directionX * chaseSpeed, rb.linearVelocity.y);
            FlipSprite(directionX);
        }
    }

    private void UpdateReturn()
    {
        // חוזר לנקודת ההתחלה, או לנקודת ה-Waypoint הראשונה אם יש כזו
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
        else if (directionX < 0 && isFacingRight)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}