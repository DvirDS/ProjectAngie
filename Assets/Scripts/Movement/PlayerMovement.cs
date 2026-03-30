using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputReader input;
    [SerializeField] private HealthDrainSystem healthDrain;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite;

    [Tooltip("רפרנס למערכת ההתגנבות שיצרנו")]
    [SerializeField] private PlayerStealth playerStealth;

    [Header("Skills")]
    [Tooltip("גרור לכאן את קובץ הסקיל של הקפיצה הכפולה מתיקיית ה-Assets")]
    [SerializeField] private Skill _doubleJumpSkillData;

    [Header("Speeds & Physics")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpForce = 8f;

    [Tooltip("כמה חזקה תהיה הקפיצה הכפולה ביחס לרגילה? (1 = אותו כוח, 0.8 = קצת פחות)")]
    [SerializeField] private float doubleJumpMultiplier = 1f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.2f;
    [SerializeField] private LayerMask groundMask;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canDoubleJump; // המשתנה החדש שזוכר אם אפשר לקפוץ שוב באוויר
    private bool isFacingRight = false;
    private float horizontalInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!animator) animator = GetComponent<Animator>();
        if (!sprite) sprite = GetComponent<SpriteRenderer>();

        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void OnEnable()
    {
        if (GameManager.I != null) GameManager.I.OnStateChanged += HandleStateChanged;
        if (input != null) input.OnJumpPressed += PerformJump;
    }

    private void OnDisable()
    {
        if (GameManager.I != null) GameManager.I.OnStateChanged -= HandleStateChanged;
        if (input != null) input.OnJumpPressed -= PerformJump;
    }

    private void Update()
    {
        if (IsGamePaused()) return;

        horizontalInput = input.Move.x;
        CheckGround();
        HandleFlip();
        UpdateVisualsAndHealth();
    }

    private void FixedUpdate()
    {
        if (IsGamePaused()) return;
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        float targetSpeed = input.SprintHeld ? sprintSpeed : walkSpeed;
        rb.linearVelocity = new Vector2(horizontalInput * targetSpeed, rb.linearVelocity.y);
    }

    // --- מערכת הקפיצות המעודכנת ---
    private void PerformJump()
    {
        if (IsGamePaused()) return;

        if (isGrounded)
        {
            // קפיצה רגילה מהקרקע
            ExecuteJump(1f);

            // מכינים את הקפיצה הכפולה (במידה ויש לה את הסקיל)
            canDoubleJump = true;
        }
        else if (canDoubleJump && _doubleJumpSkillData != null && _doubleJumpSkillData.isPurchased)
        {
            // קפיצה כפולה באוויר!
            ExecuteJump(doubleJumpMultiplier);

            // מאפסים כדי שלא תוכל לקפוץ 3 או 4 פעמים
            canDoubleJump = false;
        }
    }

    private void ExecuteJump(float forceMultiplier)
    {
        // איפוס מהירות הנפילה לפני הקפיצה כדי שהקפיצה תמיד תהיה באותו גובה
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * (jumpForce * forceMultiplier), ForceMode2D.Impulse);

        if (animator != null) animator.SetTrigger("isJumping");
    }
    // ------------------------------

    private void HandleFlip()
    {
        if (horizontalInput > 0.01f && !isFacingRight) Flip();
        else if (horizontalInput < -0.01f && isFacingRight) Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);
    }

    private void UpdateVisualsAndHealth()
    {
        bool isMoving = Mathf.Abs(horizontalInput) > 0.01f;
        bool isStealthing = (playerStealth != null && playerStealth.IsStealthing);

        if (animator != null) animator.SetBool("IsMoving", isMoving);
        if (healthDrain != null) healthDrain.SetMovementState(isMoving, input.SprintHeld, isStealthing);
    }

    private void HandleStateChanged(GameManager.GameState newState)
    {
        if (newState != GameManager.GameState.Play) rb.linearVelocity = Vector2.zero;
    }

    private bool IsGamePaused() => GameManager.I != null && GameManager.I.State != GameManager.GameState.Play;
}