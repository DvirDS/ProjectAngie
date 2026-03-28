using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Speeds")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpForce = 8f;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    [Header("Skills")]
    [SerializeField] private Skill doubleJumpSkill;
    [SerializeField] private Skill dashSkill;

    [Header("Health")]
    [SerializeField] private HealthDrainSystem healthDrain;

    [Header("Ground Check")]
    [SerializeField] private string groundTag = "Ground";

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;
    private InputSystem inputActions;

    private float moveDirectionX;
    private bool isGrounded = true;
    private bool isSprinting;
    private bool canDoubleJump;

    private bool isDashing;
    private float dashCooldownTimer;
    private float originalGravity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        inputActions = new InputSystem();
        originalGravity = rb.gravityScale;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += ctx => moveDirectionX = ctx.ReadValue<Vector2>().x;
        inputActions.Player.Move.canceled += ctx => moveDirectionX = 0f;

        inputActions.Player.Sprint.performed += ctx => isSprinting = true;
        inputActions.Player.Sprint.canceled += ctx => isSprinting = false;

        inputActions.Player.Jump.performed += ctx => OnJumpPerformed();
        inputActions.Player.Dash.performed += ctx => OnDashPerformed();

        if(GameManager.I != null)
            GameManager.I.OnStateChanged += OnGameStateChanged;

    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= ctx => moveDirectionX = ctx.ReadValue<Vector2>().x;
        inputActions.Player.Move.canceled -= ctx => moveDirectionX = 0f;

        inputActions.Player.Sprint.performed -= ctx => isSprinting = true;
        inputActions.Player.Sprint.canceled -= ctx => isSprinting = false;

        inputActions.Player.Jump.performed -= ctx => OnJumpPerformed();
        inputActions.Player.Dash.performed -= ctx => OnDashPerformed();

        inputActions.Disable();
       
        if (GameManager.I != null)
            GameManager.I.OnStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.Play)
            inputActions.Player.Enable();
        else
            inputActions.Player.Disable();
    }

    private void Update()
    {
        if (isDashing) return;

        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;

        HandleMovementAnimations();

        if (healthDrain != null)
        {
            bool isMoving = Mathf.Abs(moveDirectionX) > 0.01f;
            healthDrain.SetMovementState(isMoving, isSprinting);
        }
    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        float speed = isSprinting ? sprintSpeed : walkSpeed;
        rb.linearVelocity = new Vector2(moveDirectionX * speed, rb.linearVelocity.y);
    }

    private void OnJumpPerformed()
    {
        if (isDashing) return;

        if (isGrounded)
        {
            Jump();
        }
        else if (doubleJumpSkill != null && doubleJumpSkill.isPurchased && canDoubleJump)
        {
            Jump();
            canDoubleJump = false;
        }
    }

    private void OnDashPerformed()
    {
        if (dashSkill != null && dashSkill.isPurchased && dashCooldownTimer <= 0 && !isDashing)
            StartCoroutine(PerformDash());
    }

    private void Jump()
    {
        isGrounded = false;
        canDoubleJump = true;
        animator.SetTrigger("isJumping");
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private IEnumerator PerformDash()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;

        rb.gravityScale = 0f;
        float dashDirection = sprite.flipX ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDirection * dashForce, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        isDashing = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag(groundTag))
        {
            isGrounded = true;
            canDoubleJump = true;
        }
    }

    private void HandleMovementAnimations()
    {
        if (GameManager.I != null && GameManager.I.State != GameManager.GameState.Play) return;
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.01f;
        animator.SetBool("IsMoving", isMoving);

        if (moveDirectionX != 0)
            sprite.flipX = moveDirectionX > 0;
    }
}