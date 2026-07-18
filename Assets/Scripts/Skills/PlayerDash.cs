using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDash : MonoBehaviour
{
    [Header("Skill Connection")]
    [Tooltip("���� ���� �� ���� ����� �� ���� ������� �-Assets")]
    [SerializeField] private Skill _dashSkillData;

    [Header("References")]
    [SerializeField] private PlayerInputReader input;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    private Rigidbody2D rb;
    private bool isDashing = false;
    private float lastDashTime = -100f;
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

        rb.gravityScale = 0f;
        float dashDirection = transform.localScale.x > 0 ? -1f : 1f;

        rb.linearVelocity = new Vector2(dashDirection * dashForce, 0f);

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

    // ������ �� �������� �� ����� �� �� ����� ��������
    private void HandleDashCollision(Collision2D collision)
    {
        // 1. ������ �� ����� ������ (�����) �� ���� ������ ������
        Vector2 contactNormal = collision.GetContact(0).normal;

        // 2. ������ �� ������ ��� ����� (������ �� �����)
        // �� ���� ������ �� Y ����, �� ���� ������ ������ �� ������ �� ������ �� �����.
        // ����� ���, ��� ��������� ��� �����.
        if (Mathf.Abs(contactNormal.y) > 0.5f)
        {
            return;
        }

        DashMovable obj = collision.gameObject.GetComponent<DashMovable>();
        if (obj != null)
        {
            float dashDir = transform.localScale.x > 0 ? -1f : 1f;
            obj.ApplyPush(new Vector2(dashDir, 0));

            if (currentDashCoroutine != null) StopCoroutine(currentDashCoroutine);
            EndDash();

            rb.AddForce(new Vector2(-dashDir * 3f, 0.5f), ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing)
        {
            // ������� �� �-collision ������
            HandleDashCollision(collision);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDashing)
        {
            // ������� �� �-collision ������
            HandleDashCollision(collision);
        }
    }
}