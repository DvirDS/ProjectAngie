using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private const float DefaultWalkSpeed = 5f;
    private const float DefaultSprintSpeed = 8f;
    private const float DefaultJumpForce = 8f;
    private const float DefaultDoubleJumpMultiplier = 1f;
    private const float DefaultSpeedMultiplier = 1f;
    private const float DefaultGroundRadius = 0.2f;
    private const float ZeroVelocity = 0f;
    private const float BaseJumpMultiplier = 1f;
    private const float InputThreshold = 0.01f;
    private const float FlipInvertMultiplier = -1f;
    private const float DefaultGroundDistance = 10f;
    private const float RaycastGroundDistance = 20f;
    private const float FallProgMinDist = 10f;
    private const float FallProgMaxDist = 0.487f;
    private const float FallProgSmoothingSpeed = 15f;
    private const float MinLerpMultiplier = 0f;
    private const float MaxLerpMultiplier = 1f;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem sniffAuraEffect;

    [Header("References")]
    [SerializeField] private PlayerInputReader input;
    [SerializeField] private HealthDrainSystem healthDrain;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private PlayerStealth playerStealth;

    [Header("Skills")]
    [SerializeField] private Skill doubleJumpSkillData;

    [Header("Speeds & Physics")]
    [SerializeField] private float walkSpeed = DefaultWalkSpeed;
    [SerializeField] private float sprintSpeed = DefaultSprintSpeed;
    [SerializeField] private float jumpForce = DefaultJumpForce;
    [SerializeField] private float doubleJumpMultiplier = DefaultDoubleJumpMultiplier;
    [SerializeField] private float speedMultiplier = DefaultSpeedMultiplier;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = DefaultGroundRadius;
    [SerializeField] private LayerMask groundMask;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canDoubleJump;
    private bool isFacingRight = false;
    private float horizontalInput;
    private float currentJumpForceMultiplier = BaseJumpMultiplier;
    private bool isPreparingToJump = false;
    private bool isDigging;
    private bool isSuperSniffing;

    public float JumpForce { get => jumpForce; set => jumpForce = value; }

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

        PlayerSniff.OnSuperSniff += HandleSuperSniff;
    }

    private void OnDisable()
    {
        if (GameManager.I != null) GameManager.I.OnStateChanged -= HandleStateChanged;
        if (input != null) input.OnJumpPressed -= PerformJump;

        PlayerSniff.OnSuperSniff -= HandleSuperSniff;
    }

    private void Update()
    {
        if (!CanProcessMovement()) return;

        horizontalInput = isDigging ? ZeroVelocity : input.Move.x;
        isDigging = input.DigHeld;

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
        if (isPreparingToJump && isGrounded)
        {
            rb.linearVelocity = new Vector2(ZeroVelocity, rb.linearVelocity.y);
            return;
        }

        bool isStealthing = playerStealth != null && playerStealth.IsStealthing;
        bool isSniffing = input.SniffHeld;
        bool canSprint = input.SprintHeld && !isStealthing && !isSniffing;
        float targetSpeed = canSprint ? sprintSpeed : walkSpeed;
        rb.linearVelocity = new Vector2(horizontalInput * targetSpeed * speedMultiplier, rb.linearVelocity.y);
    }

    private void PerformJump()
    {
        if (!CanProcessMovement()) return;

        if (isGrounded)
        {
            ExecuteJump(BaseJumpMultiplier);
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
        currentJumpForceMultiplier = forceMultiplier;

        if (isGrounded)
        {
            isPreparingToJump = true;
            if (animator != null) animator.SetTrigger("IsJumping");
        }
        else
        {
            ApplyPhysicalJump();
        }
    }

    public void ApplyPhysicalJump()
    {
        isPreparingToJump = false;

        StopVerticalMovement();
        rb.AddForce(Vector2.up * (jumpForce * currentJumpForceMultiplier), ForceMode2D.Impulse);
    }

    private void HandleFlip()
    {
        if (horizontalInput > InputThreshold && !isFacingRight) Flip();
        else if (horizontalInput < -InputThreshold && isFacingRight) Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= FlipInvertMultiplier;
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
        bool isMoving = Mathf.Abs(horizontalInput) > InputThreshold;
        bool isStealthing = (playerStealth != null && playerStealth.IsStealthing);
        bool isRunning = isMoving && input.SprintHeld && !isStealthing;
        bool isSniffing = input.SniffHeld;

        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
            animator.SetBool("IsGrounded", isGrounded);

            animator.SetBool("IsRunning", isRunning);
            animator.SetBool("IsDigging", isDigging);

            animator.SetFloat("yVelocity", rb.linearVelocity.y);

            float jumpProg = Mathf.InverseLerp(jumpForce, ZeroVelocity, rb.linearVelocity.y);
            animator.SetFloat("JumpProgress", jumpProg);

            float distanceToGround = DefaultGroundDistance;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, RaycastGroundDistance, groundMask);

            if (hit.collider != null)
            {
                distanceToGround = hit.distance;
            }

            float targetFallProg = Mathf.InverseLerp(FallProgMinDist, FallProgMaxDist, distanceToGround);
            float currentFallProg = animator.GetFloat("FallProgress");
            float smoothedFallProg = Mathf.Lerp(currentFallProg, targetFallProg, Time.deltaTime * FallProgSmoothingSpeed);

            animator.SetFloat("FallProgress", smoothedFallProg);
        }

        if (sniffAuraEffect != null)
        {
            if (isSuperSniffing && !sniffAuraEffect.isPlaying)
            {
                sniffAuraEffect.Play();
            }
            else if (!isSuperSniffing && sniffAuraEffect.isPlaying)
            {
                sniffAuraEffect.Stop();
            }
        }

        if (healthDrain != null)
        {
            healthDrain.SetMovementState(isMoving, isRunning, isStealthing, isSuperSniffing);
        }
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
        horizontalInput = ZeroVelocity;
        rb.linearVelocity = Vector2.zero;
        isDigging = false;
        UpdateIdleState();
    }

    private void StopVerticalMovement()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, ZeroVelocity);
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

    private void HandleSuperSniff(bool active)
    {
        isSuperSniffing = active;
    }

    public void SlowToStop(float duration)
    {
        StartCoroutine(SlowToStopRoutine(duration));
    }

    private IEnumerator SlowToStopRoutine(float duration)
    {
        float elapsed = ZeroVelocity;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            speedMultiplier = Mathf.Lerp(MaxLerpMultiplier, MinLerpMultiplier, elapsed / duration);
            yield return null;
        }

        speedMultiplier = MinLerpMultiplier;
        rb.linearVelocity = Vector2.zero;

        if (GameManager.I != null)
            GameManager.I.GameOver();
    }
}