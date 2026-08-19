using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class DashMovable : MonoBehaviour
{
    private const float DefaultPushDistance = 2f;
    private const float DefaultMoveSpeed = 10f;
    private const float BodyMass = 50f;
    private const float DynamicGravityScale = 3f;
    private const float ArrivalThreshold = 0.05f;
    private const float FallingVelocityThreshold = -0.5f;
    private const float BlockedVelocityThreshold = 0.1f;
    private const float MinMoveThreshold = 0.1f;
    private const float ZeroVelocity = 0f;

    private Rigidbody2D rb;
    private Coroutine currentMoveCoroutine;

    [Header("Movement Settings")]
    [SerializeField] private float pushDistance = DefaultPushDistance;
    [SerializeField] private float moveSpeed = DefaultMoveSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.mass = BodyMass;
        rb.gravityScale = DynamicGravityScale;
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

            if (distanceRemaining <= ArrivalThreshold) break;

            if (rb.linearVelocity.y < FallingVelocityThreshold) break;

            if (Mathf.Abs(rb.linearVelocity.x) < BlockedVelocityThreshold && Mathf.Abs(rb.position.x - startX) > MinMoveThreshold)
            {
                yield return new WaitForFixedUpdate();
                if (Mathf.Abs(rb.linearVelocity.x) < BlockedVelocityThreshold) break;
            }

            rb.linearVelocity = new Vector2(pushDirection * moveSpeed, rb.linearVelocity.y);

            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = new Vector2(ZeroVelocity, rb.linearVelocity.y);
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        currentMoveCoroutine = null;
    }
}