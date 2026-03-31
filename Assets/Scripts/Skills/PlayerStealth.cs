using UnityEngine;

public class PlayerStealth : MonoBehaviour
{
    [Header("Skill Connection")]
    [SerializeField] private Skill _stealthSkillData;

    [Header("References")]
    [SerializeField] private PlayerInputReader input;
    [SerializeField] private SpriteRenderer spriteRenderer;

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
        if (_stealthSkillData != null && _stealthSkillData.isPurchased)
        {
            IsStealthing = !IsStealthing;

            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = IsStealthing ? 0.5f : 1f; 
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