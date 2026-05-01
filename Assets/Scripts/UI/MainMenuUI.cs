using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private SceneField persistentSceneName;
    [SerializeField] private SceneField firstRoomSceneName;
    [SerializeField] private SceneFade fadeScreen;

    public void OnPlayPressed()
    {
        fadeScreen.enabled = true;
        StartCoroutine(LoadGame());
    }

    private IEnumerator LoadGame()
    {
        fadeScreen.FadeInCoroutine(5f);
        if (!SceneManager.GetSceneByName(persistentSceneName).isLoaded)
            yield return SceneManager.LoadSceneAsync(persistentSceneName, LoadSceneMode.Additive);
        
        yield return SceneManager.LoadSceneAsync(firstRoomSceneName, LoadSceneMode.Additive);
        

        SceneManager.UnloadSceneAsync(gameObject.scene.name);
        
    }
}