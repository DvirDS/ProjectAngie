using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private SceneFade fadeScreen;
    [SerializeField] private float holdBlackScreenDuration = 2f;
    [SerializeField] private float fadeDuration = 5f;
    [SerializeField] private SceneField[] managers;

    [Header("Game Over")]
    [SerializeField] private float slowDownDuration = 5f;
    [SerializeField] private float gameOverFadeDuration = 2f;
    [SerializeField] private SceneField mainMenuScene;

    public enum GameState { Play, Pause, Tutorial, SkillTree, GameOver }

    [SerializeField] private GameState state = GameState.Play;
    public GameState State => state;

    public event System.Action<GameState> OnStateChanged;

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
    public void StartTutorial() => SetState(GameState.Tutorial);
    public void GameOver()
    {
        if (state == GameState.GameOver) return;

        state = GameState.GameOver;
        OnStateChanged?.Invoke(state);

        StartCoroutine(GameOverSequence());
    }


    public void SetState(GameState next)
    {
        if (state == next) return;
        state = next;

        if (state != GameState.GameOver) 
            Time.timeScale = (state == GameState.Play || state == GameState.Tutorial) ? 1f : 0f;

        OnStateChanged?.Invoke(state);
    }

    private IEnumerator GameOverSequence()
    {
        float elapsed = 0f;
        float startScale = Time.timeScale;
        while (elapsed < slowDownDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(startScale, 0f, elapsed / slowDownDuration);
            yield return null;
        }
        Time.timeScale = 0f;

        yield return StartCoroutine(fadeScreen.FadeOutCoroutine(gameOverFadeDuration));

        if (RespawnManager.I != null)
            Destroy(RespawnManager.I.gameObject);
        if (CoinsManager.I != null)
            Destroy(CoinsManager.I.gameObject);
        if (SkillTreeManager.I != null)
            Destroy(SkillTreeManager.I.gameObject);

        Time.timeScale = 1f;

        SceneManager.LoadScene(mainMenuScene);
    }
}