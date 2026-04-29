using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    [Header("Intro Bubble Settings")]
    [Tooltip("גרור לכאן את הפאנל של בועת הפתיחה הגדולה")]
    [SerializeField] private GameObject introBubblePanel;
    [Header("Outro Bubble Settings")]
    [SerializeField] private GameObject outroBubblePanel; // גרור לכאן את פאנל הסיום

    [Header("Movement Unlock (New)")]
    [Tooltip("הכנס לכאן את פעולות ההליכה בלבד, כדי שאנג'י תוכל לזוז לטריגרים אחרי הפתיח")]
    [SerializeField] private InputActionReference[] movementActions;

    [Header("In-Game Tutorial Settings")]
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private GameObject tutorialPanel; // הבועית הקטנה שקופצת בטריגרים
    [SerializeField] private InputActionReference[] allTutorialActions;

    public bool isIntroFinished = false;
    public bool isGameStarted = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void OnEnable()
    {
        // מציג את הבועה הגדולה של הפתיחה ברגע שהסצנה עולה
        if (introBubblePanel != null)
        {
            introBubblePanel.SetActive(true);
        }

        // מוודא שהבועית הקטנה כבויה בהתחלה
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        // מקפיאים את המשחק והפיזיקה
        Time.timeScale = 0f;

        // חוסם את כל הפעולות (קפיצה, התקפה וכו') 
        foreach (InputActionReference action in allTutorialActions)
        {
            PlayerInputReader.instance.DisableAction(action);
        }
    }

    public void CloseIntroBubble()
    {
        if (introBubblePanel != null)
        {
            introBubblePanel.SetActive(false);
        }

        isIntroFinished = true;
        Time.timeScale = 1f;

        if (movementActions != null)
        {
            foreach (InputActionReference action in movementActions)
            {
                PlayerInputReader.instance.EnableAction(action);
            }
        }
    }

    public void Show(string message, InputActionReference actRef)
    {
        PlayerInputReader.instance.EnableAction(actRef);
        tutorialText.text = message;
        tutorialPanel.SetActive(true);
    }

    public void HideTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }

    public void ShowOutro()
    {
        if (outroBubblePanel != null)
        {
            outroBubblePanel.SetActive(true);
            Time.timeScale = 0f; // מקפיאים את המשחק בזמן הקריאה
        }
    }

    // פונקציה לסגירת בועת הסיום ותחילת המשחק
    public void CloseOutroAndStartGame()
    {
        if (outroBubblePanel != null)
        {
            outroBubblePanel.SetActive(false);
        }
        Time.timeScale = 1f;

        isGameStarted = true; // כאן אנחנו מאשרים לחיים להתחיל לרדת
    }
}