using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private InputActionReference[] allTutorialActions;

    private Coroutine showRoutine;

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
        foreach (InputActionReference action in allTutorialActions)
        {
            PlayerInputReader.instance.DisableAction(action);
        }
    }

    public void Show(string message, InputActionReference actRef)
    {
        if (actRef != null) PlayerInputReader.instance.EnableAction(actRef);
        tutorialText.text = message;
        tutorialPanel.SetActive(true);
    }

    public void StopDisplay()
    {
        if (showRoutine != null)
            StopCoroutine(showRoutine);
        showRoutine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        yield return new WaitForSecondsRealtime(displayDuration);
        tutorialPanel.SetActive(false);
        showRoutine = null;
    }
}
