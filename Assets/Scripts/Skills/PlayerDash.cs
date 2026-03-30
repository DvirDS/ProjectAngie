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
    [SerializeField] private PlayerMovement playerMovement; // כדי לכבות אותו זמנית בזמן הדאש

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 15f; // עוצמת הדאש
    [SerializeField] private float dashDuration = 0.2f; // כמה זמן הדאש נמשך
    [SerializeField] private float dashCooldown = 1f; // זמן המתנה בין דאש לדאש

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
        // אנחנו מאזינים לכפתור הדאש שהגדרת ב-Input Reader
        if (input != null) input.OnDashPressed += TryDash;
    }

    private void OnDisable()
    {
        if (input != null) input.OnDashPressed -= TryDash;
    }

    private void TryDash()
    {
        // 1. מוודאים שהסקיל נרכש בעץ הסקילים
        if (_dashSkillData != null && _dashSkillData.isPurchased)
        {
            // 2. מוודאים שאנחנו לא באמצע דאש כרגע, ושעבר מספיק זמן מהדאש הקודם (Cooldown)
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

        // מכבים זמנית את סקריפט התנועה הרגיל כדי שלא יאפס לנו את המהירות
        if (playerMovement != null) playerMovement.enabled = false;

        // מבטלים כוח משיכה כדי שאנג'י תעוף ישר ולא תיפול באלכסון
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // חישוב כיוון הדאש. מכיוון שאנג'י פונה שמאלה כברירת מחדל:
        // כשה-Scale חיובי, היא מסתכלת שמאלה (-1). כשהוא שלילי, היא ימינה (1).
        float dashDirection = transform.localScale.x > 0 ? -1f : 1f;

        // דוחפים את אנג'י בכוח!
        rb.linearVelocity = new Vector2(dashDirection * dashForce, 0f);

        // ממתינים את זמן הדאש (למשל 0.2 שניות)
        yield return new WaitForSeconds(dashDuration);

        // מחזירים הכל לקדמותו: כוח משיכה, מהירות אפס, ומדליקים את סקריפט התנועה
        rb.gravityScale = originalGravity;
        rb.linearVelocity = Vector2.zero;
        if (playerMovement != null) playerMovement.enabled = true;

        isDashing = false;
    }
}