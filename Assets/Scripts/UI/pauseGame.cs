using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    public void OnResumeButton()
    {
        // במקום לכבות את האובייקט ידנית, אנחנו מודיעים למנהל המשחק לחזור
        if (GameManager.I != null)
        {
            GameManager.I.ResumeGame();
        }
    }

    public void OnQuitToMenu()
    {
        // מחזירים את הזמן למהירות רגילה לפני הטעינה
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // וודא שזה השם המדויק של סצנת התפריט
    }
}