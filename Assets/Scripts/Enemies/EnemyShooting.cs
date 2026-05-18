using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [SerializeField] private Transform firePoint;

    private EnemySO data;
    private float fireTimer = 0f;

    public void Init(EnemySO data)
    {
        this.data = data;
    }

    public void UpdateCombat(Transform player, Rigidbody2D rb, System.Action<Vector2> moveToward)
    {
        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= data.shootingRange)
        {
            rb.linearVelocity = Vector2.zero;
            TryShoot(player);
        }
        else
        {
            moveToward(player.position);
        }
    }

    public void TryShootIfInRange(Transform player, Rigidbody2D rb)
    {
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= data.shootingRange)
            TryShoot(player);
    }

    private void TryShoot(Transform player)
    {
        fireTimer -= Time.deltaTime;
        if (fireTimer > 0f) return;

        Shoot(player);
        fireTimer = data.fireRate;
    }

    private void Shoot(Transform player)
    {
        if (data.projectilePrefab == null) return;

        Vector2 spawnPos = firePoint != null ? (Vector2)firePoint.position : (Vector2)transform.position;
        GameObject bullet = Instantiate(data.projectilePrefab, spawnPos, Quaternion.identity);

        Vector2 shootDirection = (player.position - (Vector3)spawnPos).normalized;
        Projectile bulletScript = bullet.GetComponent<Projectile>();
        if (bulletScript != null)
            bulletScript.SetDirection(shootDirection, data.projectileSpeed);
    }
}