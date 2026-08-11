using UnityEngine;
using UnityEngine.EventSystems;

public class PersistentEventSystem : MonoBehaviour
{
    private static EventSystem instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = GetComponent<EventSystem>();
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != GetComponent<EventSystem>())
        {
            Destroy(gameObject);
        }
    }
}