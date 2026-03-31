using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }

    public enum GameState { Play, Pause, SkillTree, GameOver }

    [SerializeField] private GameState state = GameState.Play;
    public GameState State => state;

    public event System.Action<GameState> OnStateChanged;

    private void Awake()
    {
        if (I != null && I != this) 
        { 
            Destroy(gameObject); 
            return; 
        }
        I = this;
    }

    public void StartGame() => SetState(GameState.Play);
    public void PauseGame() => SetState(GameState.Pause);
    public void ResumeGame() => SetState(GameState.Play);    
    public void OpenSkillTree() => SetState(GameState.SkillTree);
    public void CloseSkillTree() => SetState(GameState.Play);
    public void GameOver() => SetState(GameState.GameOver);

    public void SetState(GameState next)
    {
        if (state == next) return;
        state = next;

        Time.timeScale = (state == GameState.Play) ? 1f : 0f;

        OnStateChanged?.Invoke(state);
    }
}