using UnityEngine;

public class PlayerStealth : MonoBehaviour
{
    [Header("Skill Connection")]
    [Tooltip("גרור לכאן את קובץ הסקיל של ה-Stealth")]
    [SerializeField] private Skill _stealthSkillData;

    [Header("References")]
    [SerializeField] private PlayerInputReader input;
    [SerializeField] private SpriteRenderer spriteRenderer;

    // המשתנה ששומר האם אנחנו כרגע בהתגנבות
    public bool IsStealthing { get; private set; } = false;

    private void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (input != null) input.OnStealthPressed += ToggleStealth;
    }

    private void OnDisable()
    {
        if (input != null) input.OnStealthPressed -= ToggleStealth;
    }

    private void ToggleStealth()
    {
        // בודקים האם חיברנו את קובץ הנתונים והאם השחקן קנה את הסקיל
        if (_stealthSkillData != null && _stealthSkillData.isPurchased)
        {
            IsStealthing = !IsStealthing; // מחליף מ-true ל-false וההפך (Toggle)

            // אפקט ויזואלי מגניב: הופך את אנג'י לחצי שקופה בהתגנבות!
            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = IsStealthing ? 0.5f : 1f; // 0.5 זה חצי שקוף, 1 זה רגיל
                spriteRenderer.color = c;
            }

            Debug.Log("Stealth mode active: " + IsStealthing);
        }
        else
        {
            Debug.Log("Cannot Stealth: Skill is not purchased yet!");
        }
    }
}