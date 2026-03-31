using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameStarter : MonoBehaviour
{
    [SerializeField] private string firstRoomName = "Room_1";
    [SerializeField] private float delayBeforeEvent = 0.1f;

    IEnumerator Start()
    {
        if (!IsSceneLoaded(firstRoomName))
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(firstRoomName, LoadSceneMode.Additive);

            while (!op.isDone) yield return null;
        }

        yield return new WaitForSeconds(delayBeforeEvent);

        Events.onUnloadCreateBounds?.Invoke(firstRoomName);

        Debug.Log("Game Started: " + firstRoomName + " loaded and bounds set.");
    }

    private bool IsSceneLoaded(string name)
    {
        Scene s = SceneManager.GetSceneByName(name);
        return s.IsValid() && s.isLoaded;
    }
}