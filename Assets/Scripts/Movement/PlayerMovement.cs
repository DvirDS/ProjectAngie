using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputReader input;
    [SerializeField] private HealthDrainSystem healthDrain;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite;

    [Header("Speeds & Physics")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.2f;
    [SerializeField] private LayerMask groundMask;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isFacingRight = false; // הגדרנו false כי אנג'י מצוירת שמאלה
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
        if (GameManager.I != null)
            GameManager.I.OnStateChanged += HandleStateChanged;

        // רישום לאירוע הקפיצה החדש - פותר את השגיאה!
        if (input != null)
            input.OnJumpPressed += PerformJump;
    }

    private void OnDisable()
    {
        if (GameManager.I != null)
            GameManager.I.OnStateChanged -= HandleStateChanged;

        // ביטול רישום תקין (דרישת המרצה)
        if (input != null)
            input.OnJumpPressed -= PerformJump;
    }

    private void Update()
    {
        if (IsGamePaused()) return;

        horizontalInput = input.Move.x;
        CheckGround();
        HandleFlip();

        // מחקנו מכאן את הבדיקה של JumpPressed כי הפונקציה נקראת מה-Event
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

    private void PerformJump()
    {
        // קופצים רק אם אנחנו על הקרקע והמשחק לא בהפסקה
        if (isGrounded && !IsGamePaused())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            if (animator != null) animator.SetTrigger("isJumping");
        }
    }

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

    private void CheckGround() => isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);

    private void UpdateVisualsAndHealth()
    {
        if (animator != null) animator.SetBool("IsMoving", Mathf.Abs(horizontalInput) > 0.01f);
        if (healthDrain != null) healthDrain.SetMovementState(Mathf.Abs(horizontalInput) > 0.01f, input.SprintHeld);
    }

    private void HandleStateChanged(GameManager.GameState newState)
    {
        if (newState != GameManager.GameState.Play) rb.linearVelocity = Vector2.zero;
    }

    private bool IsGamePaused() => GameManager.I != null && GameManager.I.State != GameManager.GameState.Play;
}