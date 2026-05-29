using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{

    public void OnResumeButton()
    {
        if (GameManager.I != null)
        {
            GameManager.I.ResumeGame();
        }
    }

    public void OnRestartButton()
    {
        if (GameManager.I != null)
        {
            GameManager.I.ResumeGame();
        }

        SceneManager.LoadScene("MainMenu");
    }

    public void OnQuitToMenu()
    {
        if (GameManager.I != null)
        {
            GameManager.I.ResumeGame();
        }
        SceneManager.LoadScene("MainMenu");
    }

}