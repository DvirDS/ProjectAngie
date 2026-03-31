using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDash : MonoBehaviour
{
    [Header("Skill Connection")]
    [Tooltip("גרור לכאן את קובץ הסקיל של הדאש מתיקיית ה-Assets")]
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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (playerMovement == null) playerMovement = GetComponent<PlayerMovement>();
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
                StartCoroutine(PerformDashRoutine());
            }
        }
        else
        {
            Debug.Log("Cannot Dash: Skill is not purchased yet!");
        }
    }

    private IEnumerator PerformDashRoutine()
    {
        isDashing = true;
        lastDashTime = Time.time;

        if (playerMovement != null) playerMovement.enabled = false;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float dashDirection = transform.localScale.x > 0 ? -1f : 1f;

        rb.linearVelocity = new Vector2(dashDirection * dashForce, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        rb.linearVelocity = Vector2.zero;
        if (playerMovement != null) playerMovement.enabled = true;

        isDashing = false;
    }
}