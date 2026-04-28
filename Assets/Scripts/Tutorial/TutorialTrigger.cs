using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private TutorialSO tutorial;
    private InputActionReference tutorialAction;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // אם הפתיח עדיין לא הסתיים, תעצור כאן ואל תקפיץ את הבועית
        if (!TutorialManager.instance.isIntroFinished) return;

        tutorialAction = tutorial.keyInput;
        if (tutorialAction == null) return;

        // מדליק את הבועית
        TutorialManager.instance.Show(tutorial.tutorialDescription, tutorialAction);

        // מאזינים למקשים
        tutorialAction.action.performed += OnTutorialPerformed;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // הבועית יורדת רק כשיוצאים מהטריגר
        TutorialManager.instance.HideTutorial();

        // מפסיקים להאזין למקש כשאנג'י עוזבת את האזור
        if (tutorialAction != null)
        {
            tutorialAction.action.performed -= OnTutorialPerformed;
        }
    }

    private void OnTutorialPerformed(InputAction.CallbackContext ctx)
    {
        // מסירים את ההאזנה כדי שהפעולה לא תירשם פעמיים, אבל לא מכבים את הבועית
        tutorialAction.action.performed -= OnTutorialPerformed;
    }
}