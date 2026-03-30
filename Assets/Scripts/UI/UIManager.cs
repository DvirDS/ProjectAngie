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
        // בדיקה האם עץ הסקילים פתוח כרגע
        // אנחנו ניגשים לחלון עצמו דרך ה-instance של ה-SkillTreeManager
        bool isSkillTreeOpen = SkillTreeManager.instance != null &&
                               SkillTreeManager.instance.skillTreeWindow != null &&
                               SkillTreeManager.instance.skillTreeWindow.activeSelf;

        // לוגיקת הצגת ה-HUD (הניקוד ומד החיים):
        // דולק אם: אנחנו בזמן משחק (Play) או אם עץ הסקילים פתוח
        if (hudRoot)
        {
            hudRoot.SetActive(state == GameManager.GameState.Play || isSkillTreeOpen);
        }

        // לוגיקת תפריט ה-Pause:
        // דולק רק אם: המשחק בהפסקה (Pause) וגם עץ הסקילים סגור
        if (pausePanel)
        {
            pausePanel.SetActive(state == GameManager.GameState.Pause && !isSkillTreeOpen);
        }

        // מסך GameOver תמיד מוצג במצב GameOver
        if (gameOverPanel)
        {
            gameOverPanel.SetActive(state == GameManager.GameState.GameOver);
        }
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