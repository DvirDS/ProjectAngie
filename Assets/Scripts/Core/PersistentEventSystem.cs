using UnityEngine;
using UnityEngine.EventSystems;

public class PersistentEventSystem : MonoBehaviour
{
    private static EventSystem instance;

    void Awake()
    {
        if (instance == null)
        {
            // זה ה-EventSystem הראשון שנוצר (למשל ב-MainMenu)
            instance = GetComponent<EventSystem>();
            // דואג שהוא לא יימחק כשעוברים סצנה
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != GetComponent<EventSystem>())
        {
            // אם כבר קיים אחד כזה במשחק, נשמיד את החדש כדי שלא תהיה התנגשות
            Destroy(gameObject);
        }
    }
}