using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager I { get; private set; }

    [Header("HUD")]
    [SerializeField] private GameObject hudRoot;
    [SerializeField] private Slider healthBar;

    [Header("Screens")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    // אפשר להוסיף בהמשך victoryPanel וכו'

    private void Awake()
    {
        // יצירת Singleton תקני
        if (I == null)
        {
            I = this;
        }
        else if (I != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // הרשמה לאירועים של מצב המשחק
        if (GameManager.I != null)
        {
            GameManager.I.OnStateChanged += HandleGameStateChanged;
        }
    }

    private void Start()
    {
        // וידוא שה-UI מסונכרן למצב המשחק בהתחלה
        if (GameManager.I != null)
        {
            HandleGameStateChanged(GameManager.I.State);
        }
    }

    private void OnDisable()
    {
        // ניתוק תקין (Unsubscribe) למניעת זליגות זיכרון
        if (GameManager.I != null)
        {
            GameManager.I.OnStateChanged -= HandleGameStateChanged;
        }
    }

    private void HandleGameStateChanged(GameManager.GameState state)
    {
        bool inPlay = (state == GameManager.GameState.Play);

        if (hudRoot) hudRoot.SetActive(inPlay);
        if (pausePanel) pausePanel.SetActive(state == GameManager.GameState.Pause);
        if (gameOverPanel) gameOverPanel.SetActive(state == GameManager.GameState.GameOver);
    }

    // פונקציה זו תיקרא בכל פעם שהבריאות משתנה
    public void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    // --- פונקציות לכפתורים ב-UI (למשל כפתור Resume במסך Pause) ---
    public void OnResumeClicked()
    {
        if (GameManager.I != null)
            GameManager.I.ResumeGame();
    }

    public void OnMainMenuClicked()
    {
        Debug.Log("Return to Main Menu - Load Scene Here");
        // SceneManager.LoadScene("MainMenu");
    }
}