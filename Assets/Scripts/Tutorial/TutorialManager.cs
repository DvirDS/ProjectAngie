using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    [Header("Intro Bubble Settings")]
    [SerializeField] private GameObject introBubblePanel;
    [Header("Outro Bubble Settings")]
    [SerializeField] private GameObject outroBubblePanel; 

    [Header("Movement Unlock (New)")]
    [SerializeField] private InputActionReference[] movementActions;

    [Header("In-Game Tutorial Settings")]
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private GameObject tutorialPanel; 
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
        if (introBubblePanel != null)
        {
            introBubblePanel.SetActive(true);
        }

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        Time.timeScale = 0f;

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
        if (actRef == null) return;
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
            Time.timeScale = 0f; 
        }
    }

    public void CloseOutroAndStartGame()
    {
        if (outroBubblePanel != null)
        {
            outroBubblePanel.SetActive(false);
        }
        Time.timeScale = 1f;

        isGameStarted = true; 
    }
}