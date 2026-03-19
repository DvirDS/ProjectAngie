using UnityEngine;
using UnityEngine.UI;

public class HealthDrainSystem : MonoBehaviour
{
    [Header("UI Reference")]
    public Slider healthSlider;

    [Header("Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Decay Rates (Per Second)")]
    public float passiveDecay = 0.28f;
    public float walkDecay = 0.42f;
    public float runDecay = 1.67f;

    [Header("Skills")]
    public Skill hpUpgradeSkill;
    public float hpUpgradeBonus = 50f;
    private bool hasAppliedUpgrade = false;

    // Movement state set externally by PlayerMovement
    private bool isMoving;
    private bool isSprinting;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    void Update()
    {
        float decayAmount = isSprinting ? runDecay : isMoving ? walkDecay : passiveDecay;

        currentHealth -= decayAmount * Time.deltaTime;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Debug.Log("Player is Dead / Out of Stamina");
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

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        Debug.Log("HP Upgraded and Refilled! New HP: " + maxHealth);
    }

    public void RestoreHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        Debug.Log("Bar Restored! Current value: " + currentHealth);
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        Debug.Log("Ouch! Took damage. Current health: " + currentHealth);
    }
}