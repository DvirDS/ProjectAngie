using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SceneFade fadeScreen;
    [SerializeField] private float holdBlackScreenDuration = 2f;
    [SerializeField] private float fadeDuration = 5f;
    public static GameManager I { get; private set; }

    public enum GameState { Play, Pause, Tutorial, SkillTree, GameOver }

    [SerializeField] private GameState state = GameState.Play;
    public GameState State => state;

    public event System.Action<GameState> OnStateChanged;

    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    private void Start()
    {
        StartCoroutine(startGameFade());
    }

    private IEnumerator startGameFade()
    {
        yield return StartCoroutine(fadeScreen.HoldColorDuration(Color.black, holdBlackScreenDuration));
        yield return StartCoroutine(fadeScreen.FadeInCoroutine(fadeDuration));
    }

    public void StartGame() => SetState(GameState.Play);
    public void PauseGame() => SetState(GameState.Pause);
    public void ResumeGame() => SetState(GameState.Play);
    public void OpenSkillTree() => SetState(GameState.SkillTree);
    public void CloseSkillTree() => SetState(GameState.Play);
    public void GameOver() => SetState(GameState.GameOver);
    public void StartTutorial() => SetState(GameState.Tutorial);

    public void SetState(GameState next)
    {
        if (state == next) return;
        state = next;

        Time.timeScale = (state == GameState.Play || state == GameState.Tutorial) ? 1f : 0f;

        OnStateChanged?.Invoke(state);
    }
}