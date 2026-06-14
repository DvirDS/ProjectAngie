using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AngieLightLayerModifier : MonoBehaviour
{
    [Tooltip("הכנס כאן את השם המדויק של ה-Sorting Layer של הדמות")]
    [SerializeField] private string targetLayerName = "Player";

    private int[] defaultSortingLayers;
    private Light2D angieLight;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // אנחנו משתמשים במופע שכבר קיים בסקריפט המקורי שלכם
        angieLight = LightFollowAngie.Instance;

        if (angieLight != null)
        {
            // שומרים את המצב הקיים של השכבות (למשל "Everything") כדי שנוכל להחזיר אותו אחר כך
            defaultSortingLayers = angieLight.targetSortingLayers;

            // מוצאים את תעודת הזהות (ID) של שכבת הדמות בלבד
            int playerLayerID = SortingLayer.NameToID(targetLayerName);

            // מגדירים לאור להאיר אך ורק את השכבה הזו
            angieLight.targetSortingLayers = new int[] { playerLayerID };
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // ברגע שיוצאים מהאזור, מחזירים את האור למצב ההתחלתי שלו
        if (angieLight != null && defaultSortingLayers != null)
        {
            angieLight.targetSortingLayers = defaultSortingLayers;
        }
    }
}