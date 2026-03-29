using UnityEngine;
using System; // חובה בשביל ה-Action

public class HealthDrainSystem : MonoBehaviour
{
    // הגדרת Event חדש שמשדר את הבריאות (נוכחי, מקסימום)
    public event Action<float, float> OnHealthChanged;

    [Header("Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Decay Rates (Per Second)")]
    public float passiveDecay = 0.28f;
    public float walkDecay = 0.42f;
    public float runDecay = 1.67f;

    [Header("Skills")]
    public float hpUpgradeBonus = 50f;
    private bool hasAppliedUpgrade = false;

    private bool isMoving;
    private bool isSprinting;

    void Start()
    {
        currentHealth = maxHealth;
        // עדכון ראשוני ל-UI
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    void Update()
    {
        if (GameManager.I != null && GameManager.I.State != GameManager.GameState.Play) return;

        float decayAmount = isSprinting ? runDecay : isMoving ? walkDecay : passiveDecay;
        float previousHealth = currentHealth;

        currentHealth -= decayAmount * Time.deltaTime;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // אם הבריאות השתנתה, נשדר את האירוע
        if (currentHealth != previousHealth)
        {
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Player is Dead / Out of Stamina");
            if (GameManager.I != null) GameManager.I.GameOver(); // מעבר למסך הפסד
        }
    }

    public void SetMovementState(bool moving, bool sprinting)
    {
        isMoving = moving;
        isSprinting = sprinting;
    }

    public void ApplyHpUpgrade()
    {
        if (hasAppliedUpgrade) return;
        hasAppliedUpgrade = true;
        maxHealth += hpUpgradeBonus;
        currentHealth = maxHealth;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void RestoreHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}