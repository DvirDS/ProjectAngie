using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    private const float DefaultRestoreAmount = 20f;

    [Header("Settings")]
    public float restoreAmount = DefaultRestoreAmount;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthDrainSystem playerHealth = other.GetComponent<HealthDrainSystem>();

            if (playerHealth != null)
            {
                playerHealth.RestoreHealth(restoreAmount);

                Destroy(gameObject);
            }
        }
    }
}