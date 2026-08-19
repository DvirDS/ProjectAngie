using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadTrigger : MonoBehaviour
{
    private const int FirstIndex = 0;

    [SerializeField] private SceneField[] sceneToLoad;
    [SerializeField] private SceneField[] sceneToUnload;

    private const string playerTag = "Player";
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;
        if (!collision.CompareTag(playerTag)) return;

        triggered = true;

        string sceneName = gameObject.scene.name;
        Events.onUnloadCreateBounds?.Invoke(sceneName);

        gameObject.SetActive(false);

        LoadScenes();
        UnloadScenes();
    }

    private void LoadScenes()
    {
        for (int i = FirstIndex; i < sceneToLoad.Length; i++)
        {
            bool isSceneLoaded = false;

            for (int j = FirstIndex; j < SceneManager.sceneCount; j++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(j);
                if (loadedScene.name == sceneToLoad[i].SceneName)
                {
                    isSceneLoaded = true;
                    break;
                }
            }

            if (!isSceneLoaded)
            {
                SceneManager.LoadSceneAsync(sceneToLoad[i], LoadSceneMode.Additive);
            }
        }
    }

    private void UnloadScenes()
    {
        for (int i = FirstIndex; i < sceneToUnload.Length; i++)
        {
            for (int j = FirstIndex; j < SceneManager.sceneCount; j++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(j);
                if (loadedScene.name == sceneToUnload[i].SceneName)
                {
                    SceneManager.UnloadSceneAsync(sceneToUnload[i]);
                    break;
                }
            }
        }
    }
}