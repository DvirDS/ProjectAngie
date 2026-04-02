using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputReader : MonoBehaviour
{
    private InputSystem actions;

    public event Action OnJumpPressed;
    public event Action OnSkillMenuPressed;
    public event Action OnDashPressed;
    public event Action OnStealthPressed;
    public event Action OnPausePressed; 

    public Vector2 Move { get; private set; }
    public bool SprintHeld { get; private set; }
    public bool SniffHeld { get; private set; }

    public static PlayerInputReader Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        actions = new InputSystem();
    }

    void OnEnable()
    {
        actions.Player.Move.performed += OnMove;
        actions.Player.Move.canceled += OnMove;
        actions.Player.Jump.performed += OnJump;
        actions.Player.Dash.performed += OnDash;
        actions.Player.Sprint.performed += OnSprint;
        actions.Player.Sprint.canceled += OnSprint;
        actions.Player.Sniff.performed += OnSniff;
        actions.Player.Sniff.canceled += OnSniff;
        actions.Player.Stealth.performed += OnStealth;
        actions.Player.SkillMenu.performed += OnSkillMenu;
        actions.Player.Pause.performed += OnPause;

        actions.Enable();
    }

    void OnDisable()
    {
        actions.Player.Move.performed -= OnMove;
        actions.Player.Move.canceled -= OnMove;
        actions.Player.Jump.performed -= OnJump;
        actions.Player.Dash.performed -= OnDash;
        actions.Player.Sprint.performed -= OnSprint;
        actions.Player.Sprint.canceled -= OnSprint;
        actions.Player.Sniff.performed -= OnSniff;
        actions.Player.Sniff.canceled -= OnSniff;
        actions.Player.Stealth.performed -= OnStealth;
        actions.Player.SkillMenu.performed -= OnSkillMenu;
        actions.Player.Pause.performed -= OnPause;

        actions.Disable();
    }

    public void DisableAllActions() => actions.Player.Disable();
    public void EnableAllActions() => actions.Player.Enable();
    private void OnMove(InputAction.CallbackContext ctx) => Move = ctx.ReadValue<Vector2>();
    private void OnJump(InputAction.CallbackContext ctx) => OnJumpPressed?.Invoke();
    private void OnDash(InputAction.CallbackContext ctx) => OnDashPressed?.Invoke();
    private void OnSprint(InputAction.CallbackContext ctx) => SprintHeld = ctx.performed;
    private void OnSniff(InputAction.CallbackContext ctx) => SniffHeld = ctx.performed;
    private void OnStealth(InputAction.CallbackContext ctx) => OnStealthPressed?.Invoke();
    private void OnSkillMenu(InputAction.CallbackContext ctx) => OnSkillMenuPressed?.Invoke();
    private void OnPause(InputAction.CallbackContext ctx) => OnPausePressed?.Invoke();
}