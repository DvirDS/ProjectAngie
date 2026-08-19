using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDash : MonoBehaviour
{
    private const float DefaultDashForce = 15f;
    private const float DefaultDashDuration = 0.2f;
    private const float DefaultDashCooldown = 1f;
    private const float InitialLastDashTime = -100f;
    private const float ZeroGravity = 0f;
    private const float ZeroVelocity = 0f;
    private const float ScaleThreshold = 0f;
    private const float LeftDirection = -1f;
    private const float RightDirection = 1f;
    private const int FirstContactIndex = 0;
    private const float VerticalCollisionThreshold = 0.5f;
    private const float BounceForceX = 3f;
    private const float BounceForceY = 0.5f;

    [Header("Skill Connection")]
    [SerializeField] private Skill _dashSkillData;

    [Header("References")]
    [SerializeField] private PlayerInputReader input;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = DefaultDashForce;
    [SerializeField] private float dashDuration = DefaultDashDuration;
    [SerializeField] private float dashCooldown = DefaultDashCooldown;

    private Rigidbody2D rb;
    private bool isDashing = false;
    private float lastDashTime = InitialLastDashTime;
    private float originalGravity;

    private Coroutine currentDashCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (playerMovement == null) playerMovement = GetComponent<PlayerMovement>();

        originalGravity = rb.gravityScale;
    }

    private void OnEnable()
    {
        if (input != null) input.OnDashPressed += TryDash;
    }

    private void OnDisable()
    {
        if (input != null) input.OnDashPressed -= TryDash;
    }

    private void TryDash()
    {
        if (_dashSkillData != null && _dashSkillData.isPurchased)
        {
            if (!isDashing && Time.time >= lastDashTime + dashCooldown)
            {
                if (currentDashCoroutine != null) StopCoroutine(currentDashCoroutine);
                currentDashCoroutine = StartCoroutine(PerformDashRoutine());
            }
        }
    }

    private IEnumerator PerformDashRoutine()
    {
        isDashing = true;
        lastDashTime = Time.time;

        if (playerMovement != null) playerMovement.enabled = false;

        rb.gravityScale = ZeroGravity;
        float dashDirection = transform.localScale.x > ScaleThreshold ? LeftDirection : RightDirection;

        rb.linearVelocity = new Vector2(dashDirection * dashForce, ZeroVelocity);

        yield return new WaitForSeconds(dashDuration);

        EndDash();
    }

    private void EndDash()
    {
        isDashing = false;
        rb.gravityScale = originalGravity;
        rb.linearVelocity = Vector2.zero;
        if (playerMovement != null) playerMovement.enabled = true;
    }

    private void HandleDashCollision(Collision2D collision)
    {
        Vector2 contactNormal = collision.GetContact(FirstContactIndex).normal;

        if (Mathf.Abs(contactNormal.y) > VerticalCollisionThreshold)
        {
            return;
        }

        DashMovable obj = collision.gameObject.GetComponent<DashMovable>();
        if (obj != null)
        {
            float dashDir = transform.localScale.x > ScaleThreshold ? LeftDirection : RightDirection;
            obj.ApplyPush(new Vector2(dashDir, ZeroVelocity));

            if (currentDashCoroutine != null) StopCoroutine(currentDashCoroutine);
            EndDash();

            rb.AddForce(new Vector2(-dashDir * BounceForceX, BounceForceY), ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing)
        {
            HandleDashCollision(collision);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDashing)
        {
            HandleDashCollision(collision);
        }
    }
}