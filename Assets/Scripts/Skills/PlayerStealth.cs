using UnityEngine;

public class PlayerStealth : MonoBehaviour
{
    [Header("Skill Connection")]
    [SerializeField] private Skill _stealthSkillData;

    [Header("References")]
    [SerializeField] private PlayerInputReader input;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool isInDarkZone = false;
    public bool IsInDarkZone
    {
        get => isInDarkZone;
        set
        {
            if (isInDarkZone == value) return;
            isInDarkZone = value;
            OnDarkZoneChanged?.Invoke(isInDarkZone);
        }
    }

    /// <summary>Raised whenever IsInDarkZone flips, so other systems can react instantly instead of polling.</summary>
    public event System.Action<bool> OnDarkZoneChanged;

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
        }
    }
}