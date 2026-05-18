using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Settings")]
    public float restoreAmount = 20f; 

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