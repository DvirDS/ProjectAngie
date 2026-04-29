using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private TutorialSO tutorial;
    private InputActionReference tutorialAction;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (!TutorialManager.instance.isIntroFinished) return;

        tutorialAction = tutorial.keyInput;
        if (tutorialAction == null) return;

        TutorialManager.instance.Show(tutorial.tutorialDescription, tutorialAction);

        tutorialAction.action.performed += OnTutorialPerformed;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        TutorialManager.instance.HideTutorial();

        if (tutorialAction != null)
        {
            tutorialAction.action.performed -= OnTutorialPerformed;
        }
    }

    private void OnTutorialPerformed(InputAction.CallbackContext ctx)
    {
        tutorialAction.action.performed -= OnTutorialPerformed;
    }
}