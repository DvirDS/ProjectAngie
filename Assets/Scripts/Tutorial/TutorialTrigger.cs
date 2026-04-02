using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private TutorialSO tutorial;
    private InputActionReference tutorialAction;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        TutorialManager.instance.Show(tutorial.tutorialDescription);

        tutorialAction = tutorial.keyInput;
        if (tutorialAction != null)
        {
            tutorialAction.action.performed += OnTutorialPerformed;
        }
    }

    private void OnTutorialPerformed(InputAction.CallbackContext ctx)
    {
        tutorialAction.action.performed -= OnTutorialPerformed;
        TutorialManager.instance.StopDisplay();
    }
}