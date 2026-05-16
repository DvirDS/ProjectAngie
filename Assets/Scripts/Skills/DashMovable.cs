using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class DashMovable : MonoBehaviour
{
    private Rigidbody2D rb;
    private Coroutine currentMoveCoroutine;

    [Header("Movement Settings")]
    [Tooltip("המרחק (ביחידות Unity) שהאובייקט יידחף בכל דאש")]
    [SerializeField] private float pushDistance = 2f;
    [SerializeField] private float moveSpeed = 10f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.mass = 50f;
        rb.gravityScale = 3f; // כוח משיכה קצת יותר חזק כדי להבטיח נפילה יציבה
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // נועלים את ציר ה-X כברירת מחדל כדי שהאובייקט לא יחליק וכדי שהשחקן לא יוכל לדחוף אותו סתם בהליכה
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
    }

    public void ApplyPush(Vector2 direction)
    {
        // עוצרים דחיפה קודמת אם היא עדיין קורית
        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
        }

        currentMoveCoroutine = StartCoroutine(MoveRoutine(direction.x));
    }

    private IEnumerator MoveRoutine(float dirX)
    {
        // משחררים את הנעילה האופקית כדי לאפשר את הדחיפה הפיזיקלית
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        float startX = rb.position.x;
        float targetX = startX + (dirX * pushDistance);
        float pushDirection = Mathf.Sign(dirX);

        while (true)
        {
            float distanceRemaining = Mathf.Abs(targetX - rb.position.x);

            // 1. תנאי עצירה: הגענו למרחק הדחיפה שהגדרנו
            if (distanceRemaining <= 0.05f) break;

            // 2. תנאי עצירה: נפילה. אם האובייקט באוויר ומתחיל ליפול, נפסיק את הדחיפה כדי שייפול ישר למטה
            if (rb.linearVelocity.y < -0.5f) break;

            // 3. תנאי עצירה: התנגשות בפלטפורמה/קיר. בודקים אם אנחנו מנסים לזוז אבל המהירות הפיזית נבלמה
            if (Mathf.Abs(rb.linearVelocity.x) < 0.1f && Mathf.Abs(rb.position.x - startX) > 0.1f)
            {
                // מוודאים שזו לא עצירה רגעית של פריים בודד
                yield return new WaitForFixedUpdate();
                if (Mathf.Abs(rb.linearVelocity.x) < 0.1f) break;
            }

            // מיישמים מהירות אופקית ושומרים על המהירות האנכית (כוח המשיכה)
            rb.linearVelocity = new Vector2(pushDirection * moveSpeed, rb.linearVelocity.y);

            yield return new WaitForFixedUpdate();
        }

        // סיום התנועה: איפוס המהירות האופקית ונעילת ציר ה-X בחזרה למניעת החלקות
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        currentMoveCoroutine = null;
    }
}