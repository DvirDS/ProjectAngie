using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private SceneField persistentSceneName;
    [SerializeField] private SceneField firstRoomSceneName;

    public void OnPlayPressed()
    {
        StartCoroutine(LoadGame());
    }

    private IEnumerator LoadGame()
    {
        if (!SceneManager.GetSceneByName(persistentSceneName).isLoaded)
            yield return SceneManager.LoadSceneAsync(persistentSceneName, LoadSceneMode.Additive);

        RespawnManager.instance.SetFadeAlpha(1f);

        yield return SceneManager.LoadSceneAsync(firstRoomSceneName, LoadSceneMode.Additive);
        yield return null;

        yield return StartCoroutine(RespawnManager.instance.FadeIn());

        SceneManager.UnloadSceneAsync(gameObject.scene.name);
    }
}