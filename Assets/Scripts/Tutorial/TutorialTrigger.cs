using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] protected TutorialSO tutorial;
    private InputActionReference tutorialAction;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (!TutorialManager.I.isIntroFinished) return;

        tutorialAction = tutorial.keyInput;
        if (tutorialAction == null) return;

        TutorialManager.I.Show(tutorial.tutorialDescription, tutorial.keyInput);
        tutorial.keyInput.action.performed += OnTutorialPerformed;
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        TutorialManager.I.HideTutorial();
        Unsubscribe();
    }

    private void OnTutorialPerformed(InputAction.CallbackContext ctx)
    {
        Unsubscribe();
    }

    private void Unsubscribe()
    {
        if (tutorialAction != null)
        {
            tutorialAction.action.performed -= OnTutorialPerformed;
        }
    }
}