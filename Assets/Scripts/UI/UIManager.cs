using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager I { get; private set; }

    [Header("Player References")]
    [Tooltip("גרור לכאן את אנג'י (השחקן) מההיררכיה כדי לחבר את מד החיים")]
    [SerializeField] private HealthDrainSystem playerHealthSystem;

    [Header("HUD")]
    [SerializeField] private GameObject hudRoot;
    [SerializeField] private Slider healthBar;

    [Header("Screens")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;

    private void Awake()
    {
        if (I == null) I = this;
        else if (I != this) Destroy(gameObject);
    }

    private void OnEnable()
    {
        if (GameManager.I != null) GameManager.I.OnStateChanged += HandleGameStateChanged;

        // התיקון: אנחנו מאזינים לשינויים בחיים של השחקן!
        if (playerHealthSystem != null) playerHealthSystem.OnHealthChanged += UpdateHealthUI;
    }

    private void OnDisable()
    {
        if (GameManager.I != null) GameManager.I.OnStateChanged -= HandleGameStateChanged;

        // ניתוק האזנה
        if (playerHealthSystem != null) playerHealthSystem.OnHealthChanged -= UpdateHealthUI;
    }

    private void Start()
    {
        if (GameManager.I != null) HandleGameStateChanged(GameManager.I.State);
    }

    private void HandleGameStateChanged(GameManager.GameState state)
    {
        bool inPlay = (state == GameManager.GameState.Play);
        if (hudRoot) hudRoot.SetActive(inPlay);

        bool isSkillTreeOpen = false; // עדכן את זה חזרה לפי ה-SkillTreeManager שלך
        if (pausePanel) pausePanel.SetActive(state == GameManager.GameState.Pause && !isSkillTreeOpen);
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

    public void OnResumeClicked()
    {
        if (GameManager.I != null) GameManager.I.ResumeGame();
    }
}