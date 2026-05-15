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
        
        yield return SceneManager.LoadSceneAsync(firstRoomSceneName, LoadSceneMode.Additive);
        
        SceneManager.UnloadSceneAsync(gameObject.scene.name);
    }
}