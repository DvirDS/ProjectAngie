using UnityEngine;

public class Projectile : MonoBehaviour
{
    private const float DefaultSpeed = 10f;
    private const float DefaultDamage = 10f;
    private const float DefaultLifeTime = 3f;
    private const float ZeroRotation = 0f;

    [SerializeField] private float speed = DefaultSpeed;
    [SerializeField] private float damage = DefaultDamage;
    [SerializeField] private float lifeTime = DefaultLifeTime;

    private Vector2 direction;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(Vector2 dir, float customSpeed)
    {
        direction = dir;
        speed = customSpeed;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(ZeroRotation, ZeroRotation, angle);

        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthDrainSystem playerHealth = other.GetComponent<HealthDrainSystem>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}