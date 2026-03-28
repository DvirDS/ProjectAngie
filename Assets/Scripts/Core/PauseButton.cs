using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    public void OnClick()
    {
        if (GameManager.I.State == GameManager.GameState.Pause)
        {
            GameManager.I.ResumeGame();
        }
        else
            GameManager.I.PauseGame();
    }
}