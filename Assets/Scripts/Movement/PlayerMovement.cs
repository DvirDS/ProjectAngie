using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputReader input;
    [SerializeField] private HealthDrainSystem healthDrain;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private PlayerStealth playerStealth;

    [Header("Skills")]
    [SerializeField] private Skill doubleJumpSkillData;

    [Header("Speeds & Physics")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float doubleJumpMultiplier = 1f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.2f;
    [SerializeField] private LayerMask groundMask;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canDoubleJump;
    private bool isFacingRight = false;
    private float horizontalInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
        }

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
        if (!CanProcessMovement()) return;

        horizontalInput = input.Move.x;
        CheckGround();
        HandleFlip();
        UpdateVisualsAndHealth();
    }

    private void FixedUpdate()
    {
        if (!CanProcessMovement()) return;
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        float targetSpeed = input.SprintHeld ? sprintSpeed : walkSpeed;
        rb.linearVelocity = new Vector2(horizontalInput * targetSpeed, rb.linearVelocity.y);
    }
    private void PerformJump()
    {
        if (!CanProcessMovement()) return;

        if (isGrounded)
        {
            ExecuteJump(1f);
            canDoubleJump = true;
        }
        else if (CanDoubleJump())
        {
            ExecuteJump(doubleJumpMultiplier);
            canDoubleJump = false;
        }
    }

    private bool CanDoubleJump()
    {
        return canDoubleJump
               && doubleJumpSkillData != null
               && doubleJumpSkillData.isPurchased;
    }

    private void ExecuteJump(float forceMultiplier)
    {
        StopVerticalMovement();
        rb.AddForce(Vector2.up * (jumpForce * forceMultiplier), ForceMode2D.Impulse);

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

    private void CheckGround()
    {
        if (groundCheck == null)
        {
            isGrounded = false;
            return;
        }
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);
    }

private void UpdateVisualsAndHealth()
    {
        bool isMoving = Mathf.Abs(horizontalInput) > 0.01f;
        bool isRunning = isMoving && input.SprintHeld;
        bool isStealthing = (playerStealth != null && playerStealth.IsStealthing);
        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetBool("IsRunning", isRunning);
            
            animator.SetFloat("yVelocity", rb.linearVelocity.y);

            float jumpProg = Mathf.InverseLerp(jumpForce, 0f, rb.linearVelocity.y);
            animator.SetFloat("JumpProgress", jumpProg);

            float distanceToGround = 10f; 
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 20f, groundMask);
            
            if (hit.collider != null)
            {
                distanceToGround = hit.distance;
            }

            float targetFallProg = Mathf.InverseLerp(4f, 0.487f, distanceToGround);     
            float currentFallProg = animator.GetFloat("FallProgress");
            float smoothedFallProg = Mathf.Lerp(currentFallProg, targetFallProg, Time.deltaTime * 15f);

            animator.SetFloat("FallProgress", smoothedFallProg);
        }

        if (healthDrain != null) healthDrain.SetMovementState(isMoving, input.SprintHeld, isStealthing);
    }

    private void HandleStateChanged(GameManager.GameState newState)
    {
        if (newState != GameManager.GameState.Play)
        {
            StopMovement();
        }
    }

    private void StopMovement()
    {
        horizontalInput = 0f;
        rb.linearVelocity = Vector2.zero;
        UpdateIdleState();
    }

    private void StopVerticalMovement()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
    }

    private void UpdateIdleState()
    {
        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }

        if (healthDrain != null)
        {
            bool isStealthing = playerStealth != null && playerStealth.IsStealthing;
            healthDrain.SetMovementState(false, false, isStealthing);
        }
    }

    private bool CanProcessMovement()
    {
        if (GameManager.I == null) return true;
        return GameManager.I.State == GameManager.GameState.Play ||
               GameManager.I.State == GameManager.GameState.Tutorial;
    }
}