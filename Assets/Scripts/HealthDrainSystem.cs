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

        bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        bool isRunning = Input.GetKey(KeyCode.LeftShift); 

        float decayAmount = passiveDecay;

        if (isMoving)
        {
            if (isRunning)
            {
                decayAmount = runDecay;
            }
            else
            {
                decayAmount = walkDecay;
            }
        }

        currentHealth -= decayAmount * Time.deltaTime;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Player is Dead / Out of Stamina");
        }
    }

    public void RestoreHealth(float amount)
    {
        currentHealth += amount;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        Debug.Log("Bar Restored! Current value: " + currentHealth);
    }
}