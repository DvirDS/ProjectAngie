using UnityEngine;
using System;

public class HealthDrainSystem : MonoBehaviour
{
    public event Action<float, float> OnHealthChanged;

    [Header("Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Decay Rates (Per Second)")]
    [SerializeField] private float passiveDecay = 0.28f;
    [SerializeField] private float walkDecay = 0.42f;
    [SerializeField] private float stealthDecay = 0.8f;
    [SerializeField] private float runDecay = 1.67f;

    [Header("Skills")]
    [SerializeField] private float hpUpgradeBonus = 50f;
    [SerializeField] private bool hasAppliedUpgrade = false;

    private bool isMoving;
    private bool isSprinting;
    private bool isStealthing;

    private void Awake()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Update()
    {
        if (GameManager.I != null && GameManager.I.State != GameManager.GameState.Play) return;
        float decayAmount = passiveDecay;

        if (isSprinting && isMoving) decayAmount = runDecay;
        else if (isStealthing && isMoving) decayAmount = stealthDecay;
        else if (isMoving) decayAmount = walkDecay;
        else if (isStealthing) decayAmount = stealthDecay;

        float previousHealth = currentHealth;
        currentHealth -= decayAmount * Time.deltaTime;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth != previousHealth)
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            RestoreHealth(maxHealth);
            if (RespawnManager.instance != null)
                RespawnManager.instance.Respawn();
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