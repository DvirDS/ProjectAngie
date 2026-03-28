using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private string persistentSceneName = "PersistentGameplay";
    [SerializeField] private string firstRoomSceneName = "Room_1";

    public void OnPlayPressed()
    {
        StartCoroutine(LoadGame());
    }

    private IEnumerator LoadGame()
    {
        yield return SceneManager.LoadSceneAsync(persistentSceneName, LoadSceneMode.Additive);
        yield return SceneManager.LoadSceneAsync(firstRoomSceneName, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(gameObject.scene.name);
    }
}