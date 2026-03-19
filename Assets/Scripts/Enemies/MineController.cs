using UnityEngine;

public class MineController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float damageAmount = 30f;
    [SerializeField] private GameObject redAuraObject; 

    void Start()
    {
        if (redAuraObject != null)
        {
            redAuraObject.SetActive(false);
        }
    }

    public void SetSmellVisible(bool canSmell)
    {
        if (redAuraObject != null)
        {
            redAuraObject.SetActive(canSmell);
        }
    }

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