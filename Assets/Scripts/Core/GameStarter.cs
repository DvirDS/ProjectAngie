using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameStarter : MonoBehaviour
{
    private const string DefaultFirstRoomName = "Room_1";
    private const float DefaultDelayBeforeEvent = 0.1f;

    [SerializeField] private string firstRoomName = DefaultFirstRoomName;
    [SerializeField] private float delayBeforeEvent = DefaultDelayBeforeEvent;

    IEnumerator Start()
    {
        if (!IsSceneLoaded(firstRoomName))
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(firstRoomName, LoadSceneMode.Additive);

            while (!op.isDone) yield return null;
        }

        yield return new WaitForSeconds(delayBeforeEvent);

        Events.onUnloadCreateBounds?.Invoke(firstRoomName);
    }

    private bool IsSceneLoaded(string name)
    {
        Scene s = SceneManager.GetSceneByName(name);
        return s.IsValid() && s.isLoaded;
    }
}