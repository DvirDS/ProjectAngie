using UnityEngine;
using System;

public class HealthDrainSystem : MonoBehaviour
{
    public event Action<float, float> OnHealthChanged;

    [Header("Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Decay Rates (Per Second)")]
    public float passiveDecay = 0.28f; // עמידה במקום
    public float walkDecay = 0.42f;    // הליכה
    public float stealthDecay = 0.8f;  // התגנבות (תוכל לשנות את המספר כרצונך)
    public float runDecay = 1.67f;     // ריצה מהירה

    [Header("Skills")]
    public float hpUpgradeBonus = 50f;
    private bool hasAppliedUpgrade = false;

    // States
    private bool isMoving;
    private bool isSprinting;
    private bool isStealthing; // המצב החדש שהוספנו

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    void Update()
    {
        if (GameManager.I != null && GameManager.I.State != GameManager.GameState.Play) return;

        // קביעת קצב ירידת החיים לפי הפעולה הנוכחית של אנג'י
        float decayAmount = passiveDecay;

        if (isSprinting && isMoving) decayAmount = runDecay;
        else if (isStealthing && isMoving) decayAmount = stealthDecay;
        else if (isMoving) decayAmount = walkDecay;
        else if (isStealthing) decayAmount = stealthDecay; // התגנבות במקום עדיין מעייפת

        float previousHealth = currentHealth;
        currentHealth -= decayAmount * Time.deltaTime;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // שידור ל-UI רק אם החיים באמת השתנו
        if (currentHealth != previousHealth)
        {
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Player is Dead / Out of Stamina");
            if (GameManager.I != null) GameManager.I.GameOver();
        }
    }

    // הוספנו את ההתגנבות לפרמטרים שמקבלים מסקריפט התנועה
    public void SetMovementState(bool moving, bool sprinting, bool stealthing)
    {
        isMoving = moving;
        isSprinting = sprinting;
        isStealthing = stealthing;
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