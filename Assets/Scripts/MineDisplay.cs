using UnityEngine;

public class MineDisplay : MonoBehaviour
{
    // לכאן נגרור את הריבוע האדום (ההילה)
    public GameObject redAuraObject;

    void Start()
    {
        // ליתר ביטחון - מוודאים שההילה כבויה בתחילת המשחק
        if (redAuraObject != null)
        {
            redAuraObject.SetActive(false);
        }
    }

    // אנג'י תקרא לפונקציה הזו
    public void SetSmellVisible(bool canSmell)
    {
        if (redAuraObject != null)
        {
            // אם מריחים (true) -> מפעיל את האובייקט
            // אם לא מריחים (false) -> מכבה את האובייקט
            redAuraObject.SetActive(canSmell);
        }
    }
}