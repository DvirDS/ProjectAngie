using UnityEngine;

public class ExplosiveMine : MonoBehaviour
{
    [Header("Settings")]
    public float damageAmount = 30f; // כמה נזק המוקש עושה

    private void OnTriggerEnter2D(Collider2D other)
    {
        // בודק אם השחקן נכנס למוקש
        if (other.CompareTag("Player"))
        {
            // מנסה למצוא את מערכת החיים של השחקן
            HealthDrainSystem playerHealth = other.GetComponent<HealthDrainSystem>();

            if (playerHealth != null)
            {
                // מפעיל את הפונקציה שהוספנו בשלב 1
                playerHealth.TakeDamage(damageAmount);

                // משמיד את המוקש
                Destroy(gameObject);
            }
        }
    }
}