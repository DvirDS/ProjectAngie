using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : Singleton<TutorialManager>
{
    [Header("Bubbles")]
    [SerializeField] private GameObject introBubblePanel;
    [SerializeField] private GameObject outroBubblePanel;

    
    [Header("In-Game Tutorial")]
    [SerializeField] private Collider2D moveTrigger;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private GameObject tutorialPanel;

    [Header("All Tutorial Actions � disabled at start")]
    [SerializeField] private InputActionReference[] allTutorialActions;

    public bool isIntroFinished = false;

    private void OnEnable()
    {
        introBubblePanel?.SetActive(true);
        outroBubblePanel?.SetActive(false);
        tutorialPanel?.SetActive(false);
        moveTrigger.enabled = false;

        foreach (InputActionReference action in allTutorialActions)
            PlayerInputReader.I.DisableAction(action);

        moveTrigger.enabled = false;
    }

    public void ShowIntro()
    {
        introBubblePanel?.SetActive(true);
        GameManager.I.SetState(GameManager.GameState.Tutorial);
    }

    public void CloseIntroBubble()
    {
        introBubblePanel?.SetActive(false);
        isIntroFinished = true;

        moveTrigger.enabled = true;
        moveTrigger.isTrigger = true;
    }

    public void Show(string message, InputActionReference actRef)
    {
        if (actRef != null)
            PlayerInputReader.I.EnableAction(actRef);

        tutorialText.text = message;
        tutorialPanel.SetActive(true);
    }

    public void HideTutorial()
    {
        tutorialPanel?.SetActive(false);
    }

    public void ShowOutro()
    {
        outroBubblePanel?.SetActive(true);
        Time.timeScale = 0f;
        foreach (InputActionReference action in allTutorialActions)
            PlayerInputReader.I.DisableAction(action);
    }

    public void CloseOutroAndStartGame()
    {
        outroBubblePanel?.SetActive(false);
        foreach (InputActionReference action in allTutorialActions)
            PlayerInputReader.I.EnableAction(action);
        
        GameManager.I.SetState(GameManager.GameState.Play);
    }
}