using UnityEngine;

public class EnemyFlying : MonoBehaviour
{
    private EnemySO data;
    private Vector2 startPosition;

    public void Init(Rigidbody2D rb, EnemySO data)
    {
        this.data = data;
        startPosition = transform.position;
        rb.gravityScale = 0f;
    }

    public bool IsOutsideBoundary(Vector2 enemyPos, Vector2 playerPos)
    {
        float enemyDist = Vector2.Distance(startPosition, enemyPos);
        float playerDist = Vector2.Distance(startPosition, playerPos);
        return enemyDist >= data.maxChaseDistance && playerDist > data.maxChaseDistance;
    }
}