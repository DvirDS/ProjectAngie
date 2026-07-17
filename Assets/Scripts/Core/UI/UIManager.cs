using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager I { get; private set; }

    [Header("Player References")]
    [SerializeField] private HealthDrainSystem playerHealthSystem;
    [SerializeField] private PlayerInputReader inputReader;

    [Header("HUD Elements")]
    [SerializeField] private GameObject healthBarRoot;
    [SerializeField] private GameObject scoreRoot;
    [SerializeField] private Slider healthBar;

    [Header("Damage Flash Settings")]
    [SerializeField] private Image damageFlashImage;
    [SerializeField] private float flashDuration = 0.25f; 
    [SerializeField] private Color flashColor = new Color(1f, 0f, 0f, 1f); 

    private Color originalColor; 
    private Coroutine flashCoroutine;

    [Header("Screens")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject skillTreePanel;

    private void Awake()
    {
        if (I == null) I = this;
        else if (I != this) Destroy(gameObject);

        
        if (damageFlashImage != null)
        {
            originalColor = damageFlashImage.color;
        }
    }

    private void OnEnable()
    {
        if (GameManager.I != null) GameManager.I.OnStateChanged += HandleGameStateChanged;
        if (inputReader != null) inputReader.OnEscPressed += TogglePause;

        if (playerHealthSystem != null)
        {
            playerHealthSystem.OnHealthChanged += UpdateHealthUI;
            playerHealthSystem.OnDamageTaken += TriggerDamageFlash;
        }
    }

    private void OnDisable()
    {
        if (GameManager.I != null) GameManager.I.OnStateChanged -= HandleGameStateChanged;
        if (inputReader != null) inputReader.OnEscPressed -= TogglePause;

        if (playerHealthSystem != null)
        {
            playerHealthSystem.OnHealthChanged -= UpdateHealthUI;
            playerHealthSystem.OnDamageTaken -= TriggerDamageFlash;
        }
    }

    private void Start()
    {
        if (GameManager.I != null) HandleGameStateChanged(GameManager.I.State);
    }

    private void TogglePause()
    {
        if (GameManager.I == null) return;
        if (GameManager.I.State == GameManager.GameState.Play)
            GameManager.I.PauseGame();
        else if (GameManager.I.State == GameManager.GameState.Pause)
            GameManager.I.ResumeGame();
    }

    private void HandleGameStateChanged(GameManager.GameState state)
    {
        if (healthBarRoot) healthBarRoot.SetActive(state == GameManager.GameState.Play ||
            state == GameManager.GameState.Tutorial);
        if (pausePanel) pausePanel.SetActive(state == GameManager.GameState.Pause);
        if (skillTreePanel) skillTreePanel.SetActive(state == GameManager.GameState.SkillTree);
        if (gameOverPanel) gameOverPanel.SetActive(state == GameManager.GameState.GameOver);
    }

    public void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    private void TriggerDamageFlash()
    {
        if (damageFlashImage == null) return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        damageFlashImage.color = flashColor;

        float elapsedTime = 0f;

        while (elapsedTime < flashDuration)
        {
            elapsedTime += Time.deltaTime;
            damageFlashImage.color = Color.Lerp(flashColor, originalColor, elapsedTime / flashDuration);
            yield return null;
        }

        damageFlashImage.color = originalColor;
    }

    public void OnResumeClicked()
    {
        if (GameManager.I != null) GameManager.I.ResumeGame();
    }

    public void OnRestartButton()
    {
        GameManager.I?.GameOver();
    }
}