using UnityEngine;
using System;

public class PlayerSniff : MonoBehaviour
{
    // שני ערוצי השידור שלנו!
    public static event Action<bool> OnNormalSniff;
    public static event Action<bool> OnSuperSniff;

    [Header("Skill Connection")]
    [Tooltip("גרור לכאן את קובץ הסקיל של ה-Super Sniff מתיקיית ה-Assets")]
    [SerializeField] private Skill _superSniffSkillData;

    [Header("References")]
    [SerializeField] private PlayerInputReader input;

    private bool isSniffing = false;

    private void Update()
    {
        // האם השחקן מחזיק כרגע את הכפתור?
        bool isHoldingButton = input.SniffHeld;

        // אם יש שינוי במצב (התחיל לרחרח או עזב)
        if (isHoldingButton != isSniffing)
        {
            isSniffing = isHoldingButton;

            // 1. מפעילים את הרחרוח הרגיל (תמיד עובד)
            OnNormalSniff?.Invoke(isSniffing);

            // 2. מפעילים את רחרוח העל (רק אם הסקיל נרכש!)
            bool hasSuperSniff = _superSniffSkillData != null && _superSniffSkillData.isPurchased;

            if (hasSuperSniff)
            {
                OnSuperSniff?.Invoke(isSniffing);
            }
            else
            {
                // אם עזבנו את הכפתור, או שאין לנו את הסקיל, נשדר "כיבוי" ליתר ביטחון
                OnSuperSniff?.Invoke(false);
            }
        }
    }
}