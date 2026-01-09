using UnityEngine;

public class ExplosiveMine : MonoBehaviour
{
    [Header("Settings")]
    public float damageAmount = 30f; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Player"))
        {
            
            HealthDrainSystem playerHealth = other.GetComponent<HealthDrainSystem>();

            if (playerHealth != null)
            {
                
                playerHealth.TakeDamage(damageAmount);

                
                Destroy(gameObject);
            }
        }
    }
}