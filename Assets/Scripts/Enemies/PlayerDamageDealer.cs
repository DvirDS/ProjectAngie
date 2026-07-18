using UnityEngine;

public class PlayerDamageDealer : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float interval = 1f;
    [SerializeField] private bool destroyOnHit = false;
    [SerializeField] private float destroyDelay = 0f;

    private float lastHitTime = -Mathf.Infinity;
    private bool hasDealtLethalHit = false;

    public void Initialize(float damage, float interval)
    {
        this.damage = damage;
        this.interval = interval;
    }

    private void OnTriggerEnter2D(Collider2D other) => TryDamage(other);
    private void OnTriggerStay2D(Collider2D other) => TryDamage(other);

    private void TryDamage(Collider2D other)
    {
        if (hasDealtLethalHit) return;
        if (!other.CompareTag("Player")) return;

        if (Time.time < lastHitTime + interval) return;

        HealthDrainSystem playerHealth = other.GetComponent<HealthDrainSystem>();
        if (playerHealth == null) return;

        playerHealth.TakeDamage(damage);
        lastHitTime = Time.time;

        if (destroyOnHit)
        {
            hasDealtLethalHit = true;

            Collider2D hitCollider = GetComponent<Collider2D>();
            if (hitCollider != null) hitCollider.enabled = false;

            Destroy(gameObject, destroyDelay);
        }
    }
}
