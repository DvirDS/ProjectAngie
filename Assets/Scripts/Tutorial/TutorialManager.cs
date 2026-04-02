using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private float displayDuration = 3f;

    private Coroutine showRoutine;

    private void Awake()
    {
        if (instance == null) 
            instance = this;
    }

    public void Show(string message)
    {
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
