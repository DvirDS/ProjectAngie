using UnityEngine;

public class PlayerDamageDealer : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float interval = 0f; // 0 = one hit, like mines
    [SerializeField] private bool destroyOnHit = false;

    private float lastHitTime = -Mathf.Infinity;

    public void Initialize(float damage, float interval)
    {
        this.damage = damage;
        this.interval = interval;
    }

    private void OnTriggerEnter2D(Collider2D other) => TryDamage(other);
    private void OnTriggerStay2D(Collider2D other) => TryDamage(other);

    private void TryDamage(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time < lastHitTime + interval) return;

        HealthDrainSystem playerHealth = other.GetComponent<HealthDrainSystem>();
        if (playerHealth == null) return;

        playerHealth.TakeDamage(damage);
        lastHitTime = Time.time;

        if (destroyOnHit) Destroy(gameObject);
    }
}