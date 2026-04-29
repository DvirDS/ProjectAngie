using UnityEngine;
using System;

public class HealthDrainSystem : MonoBehaviour
{
    public event Action<float, float> OnHealthChanged;

    [Header("Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Decay Rates (Per Second)")]
    public float passiveDecay = 0.28f; 
    public float walkDecay = 0.42f;    
    public float stealthDecay = 0.8f; 
    public float runDecay = 1.67f;     

    [Header("Skills")]
    public float hpUpgradeBonus = 50f;
    private bool hasAppliedUpgrade = false;

    private bool isMoving;
    private bool isSprinting;
    private bool isStealthing; 

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    void Update()
    {
        // בדיקה 1: האם ה-GameManager במצב Play
        if (GameManager.I != null && GameManager.I.State != GameManager.GameState.Play) return;

        // בדיקה 2 (החדשה): האם אנחנו בתוך ה-Tutorial?
        // אם המנהל קיים והמשחק עוד לא התחיל רשמית - אנחנו לא מורידים חיים
        if (TutorialManager.instance != null && !TutorialManager.instance.isGameStarted) return;

        // ... שאר הקוד המצוין שלך של חישוב ה-decayAmount ...
        float decayAmount = passiveDecay;

        if (isSprinting && isMoving) decayAmount = runDecay;
        else if (isStealthing && isMoving) decayAmount = stealthDecay;
        else if (isMoving) decayAmount = walkDecay;
        else if (isStealthing) decayAmount = stealthDecay; 

        float previousHealth = currentHealth;
        currentHealth -= decayAmount * Time.deltaTime;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

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