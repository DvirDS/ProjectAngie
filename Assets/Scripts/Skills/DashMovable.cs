using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class DashMovable : MonoBehaviour
{
    private Rigidbody2D rb;
    private Coroutine currentMoveCoroutine;

    [Header("Movement Settings")]
    [SerializeField] private float pushDistance = 2f;
    [SerializeField] private float moveSpeed = 10f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.mass = 50f;
        rb.gravityScale = 3f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
    }

    public void ApplyPush(Vector2 direction)
    {
        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
        }

        currentMoveCoroutine = StartCoroutine(MoveRoutine(direction.x));
    }

    private IEnumerator MoveRoutine(float dirX)
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        float startX = rb.position.x;
        float targetX = startX + (dirX * pushDistance);
        float pushDirection = Mathf.Sign(dirX);

        while (true)
        {
            float distanceRemaining = Mathf.Abs(targetX - rb.position.x);

            if (distanceRemaining <= 0.05f) break;

            if (rb.linearVelocity.y < -0.5f) break;

            if (Mathf.Abs(rb.linearVelocity.x) < 0.1f && Mathf.Abs(rb.position.x - startX) > 0.1f)
            {
                yield return new WaitForFixedUpdate();
                if (Mathf.Abs(rb.linearVelocity.x) < 0.1f) break;
            }

            rb.linearVelocity = new Vector2(pushDirection * moveSpeed, rb.linearVelocity.y);

            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        currentMoveCoroutine = null;
    }
}