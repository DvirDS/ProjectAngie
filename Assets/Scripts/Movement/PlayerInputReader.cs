using UnityEngine;
using UnityEngine.InputSystem;
using System; // חובה עבור Action

public class PlayerInputReader : MonoBehaviour
{
    private InputSystem actions;

    // --- Events (הדרך המקצועית והבטוחה) ---
    public event Action OnJumpPressed;
    public event Action OnSkillMenuPressed;
    public event Action OnDashPressed;

    // --- משתנים למצב "לחוץ" (Held) ---
    public Vector2 Move { get; private set; }
    public bool SprintHeld { get; private set; }
    public bool SniffHeld { get; private set; }

    void Awake()
    {
        actions = new InputSystem();
    }

    void OnEnable()
    {
        // הרשמה תקינה לאירועים ללא למבדות
        actions.Player.Move.performed += OnMove;
        actions.Player.Move.canceled += OnMove;

        actions.Player.Jump.performed += OnJump;
        actions.Player.Dash.performed += OnDash;

        actions.Player.Sprint.performed += OnSprint;
        actions.Player.Sprint.canceled += OnSprint;

        actions.Player.Sniff.performed += OnSniff;
        actions.Player.Sniff.canceled += OnSniff;

        actions.Player.SkillMenu.performed += OnSkillMenu;

        actions.Enable();
    }

    void OnDisable()
    {
        // הסרת רישום (Unsubscribe) תקנית למניעת זליגות זיכרון
        actions.Player.Move.performed -= OnMove;
        actions.Player.Move.canceled -= OnMove;

        actions.Player.Jump.performed -= OnJump;
        actions.Player.Dash.performed -= OnDash;

        actions.Player.Sprint.performed -= OnSprint;
        actions.Player.Sprint.canceled -= OnSprint;

        actions.Player.Sniff.performed -= OnSniff;
        actions.Player.Sniff.canceled -= OnSniff;

        actions.Player.SkillMenu.performed -= OnSkillMenu;

        actions.Disable();
    }

    // --- פונקציות העזר שמפעילות את ה-Events ---
    private void OnMove(InputAction.CallbackContext ctx) => Move = ctx.ReadValue<Vector2>();

    private void OnJump(InputAction.CallbackContext ctx)
    {
        // מפעיל את ה-Event רק אם מישהו נרשם אליו
        OnJumpPressed?.Invoke();
    }

    private void OnDash(InputAction.CallbackContext ctx)
    {
        OnDashPressed?.Invoke();
    }

    private void OnSprint(InputAction.CallbackContext ctx)
    {
        SprintHeld = ctx.performed;
    }

    private void OnSniff(InputAction.CallbackContext ctx)
    {
        SniffHeld = ctx.performed;
    }

    private void OnSkillMenu(InputAction.CallbackContext ctx)
    {
        OnSkillMenuPressed?.Invoke();
    }
}