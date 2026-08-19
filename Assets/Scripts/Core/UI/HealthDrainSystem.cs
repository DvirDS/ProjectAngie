using UnityEngine;
using System;
using Unity.VisualScripting;

public class HealthDrainSystem : MonoBehaviour
{
    private const float DefaultMaxHealth = 100f;
    private const float DefaultPassiveDecay = 0.28f;
    private const float DefaultWalkDecay = 0.42f;
    private const float DefaultStealthDecay = 0.8f;
    private const float DefaultRunDecay = 1.67f;
    private const float DefaultSniffDecay = 1.2f;
    private const float DefaultHpUpgradeBonus = 50f;
    private const float MinHealth = 0f;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDamageTaken;

    [Header("Settings")]
    [SerializeField] private float maxHealth = DefaultMaxHealth;
    [SerializeField] private float currentHealth;

    [Header("Decay Rates (Per Second)")]
    [SerializeField] private float passiveDecay = DefaultPassiveDecay;
    [SerializeField] private float walkDecay = DefaultWalkDecay;
    [SerializeField] private float stealthDecay = DefaultStealthDecay;
    [SerializeField] private float runDecay = DefaultRunDecay;
    [SerializeField] private float sniffDecay = DefaultSniffDecay;

    [Header("Skills")]
    [SerializeField] private float hpUpgradeBonus = DefaultHpUpgradeBonus;
    [SerializeField] private bool hasAppliedUpgrade = false;

    private bool isMoving;
    private bool isSprinting;
    private bool isStealthing;
    private bool isSniffing;
    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Update()
    {
        if (GameManager.I != null && GameManager.I.State != GameManager.GameState.Play) return;
        if (isDead) return;

        float decayAmount = isStealthing ? stealthDecay
                          : isSniffing ? sniffDecay
                          : (isSprinting && isMoving) ? runDecay
                          : isMoving ? walkDecay
                          : passiveDecay;

        float previousHealth = currentHealth;
        currentHealth -= decayAmount * Time.deltaTime;
        currentHealth = Mathf.Clamp(currentHealth, MinHealth, maxHealth);

        if (currentHealth != previousHealth)
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= MinHealth && !isDead)
        {
            isDead = true;
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            if (RespawnManager.I != null)
                RespawnManager.I.Respawn();
        }
    }

    public void SetMovementState(bool moving, bool sprinting, bool stealthing, bool sniffing = false)
    {
        isMoving = moving;
        isSprinting = sprinting;
        isStealthing = stealthing;
        isSniffing = sniffing;
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
        currentHealth = Mathf.Clamp(currentHealth + amount, MinHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, MinHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth > MinHealth)
        {
            OnDamageTaken?.Invoke();
        }
    }

    public void ResetAfterRespawn()
    {
        isDead = false;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}