using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameStarter : MonoBehaviour
{
    [SerializeField] private string firstRoomName = "Room_1";
    [SerializeField] private float delayBeforeEvent = 0.1f;

    IEnumerator Start()
    {
        // 1. בודק אם החדר הראשון כבר טעון (למשל אם גררת אותו ב-Editor)
        if (!IsSceneLoaded(firstRoomName))
        {
            // טוען את החדר הראשון במצב Additive
            AsyncOperation op = SceneManager.LoadSceneAsync(firstRoomName, LoadSceneMode.Additive);

            // ממתין עד שהטעינה תסתיים
            while (!op.isDone) yield return null;
        }

        // 2. ממתין שבריר שנייה כדי לוודא שכל ה-Awake בחדר החדש הסתיימו
        yield return new WaitForSeconds(delayBeforeEvent);

        // 3. מפעיל את האירוע שהשותפה שלך בנתה כדי שהמצלמה תתביית על החדר
        // זה יפעיל את turnRoomColliderOn בתוך CreateRoomBounds של החדר הראשון
        Events.onUnloadCreateBounds?.Invoke(firstRoomName);

        Debug.Log("Game Started: " + firstRoomName + " loaded and bounds set.");
    }

    private bool IsSceneLoaded(string name)
    {
        Scene s = SceneManager.GetSceneByName(name);
        return s.IsValid() && s.isLoaded;
    }
}