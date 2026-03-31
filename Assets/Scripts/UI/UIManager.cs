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
        if (playerHealthSystem != null) playerHealthSystem.OnHealthChanged += UpdateHealthUI;
        if (inputReader != null) inputReader.OnPausePressed += TogglePause;
    }

    private void OnDisable()
    {
        if (GameManager.I != null) GameManager.I.OnStateChanged -= HandleGameStateChanged;
        if (playerHealthSystem != null) playerHealthSystem.OnHealthChanged -= UpdateHealthUI;
        if (inputReader != null) inputReader.OnPausePressed -= TogglePause;
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
        bool isPlay = (state == GameManager.GameState.Play);
        bool isSkillTreeOpen = SkillTreeManager.instance != null &&
                               SkillTreeManager.instance.skillTreeWindow != null &&
                               SkillTreeManager.instance.skillTreeWindow.activeSelf;

        if (healthBarRoot) healthBarRoot.SetActive(isPlay);

        if (scoreRoot) scoreRoot.SetActive(isPlay || isSkillTreeOpen);

        if (pausePanel)
            pausePanel.SetActive(state == GameManager.GameState.Pause && !isSkillTreeOpen);

        if (gameOverPanel)
            gameOverPanel.SetActive(state == GameManager.GameState.GameOver);
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