using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadTrigger : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string sceneToUnload;
    private const string playerTag = "Player";
    private string sceneName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            sceneName = gameObject.scene.name;
            Events.onUnloadCreateBounds?.Invoke(sceneName);
            Collider2D collider = GetComponent<Collider2D>();
            collider.gameObject.SetActive(false);

            LoadScene();
            UnloadScene();
        } 
    }

    private void LoadScene()
    {
        if (!string.IsNullOrWhiteSpace(sceneToLoad) && !IsSceneLoaded(sceneToLoad))
        {
            SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        }
    }

    private void UnloadScene()
    {
        if (!string.IsNullOrWhiteSpace(sceneToUnload) && IsSceneLoaded(sceneToUnload))
        {
            SceneManager.UnloadSceneAsync(sceneToUnload);
        }
    }

    private static bool IsSceneLoaded(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        return scene.IsValid() && scene.isLoaded;
    }
}
