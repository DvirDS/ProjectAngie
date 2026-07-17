using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInputReader : MonoBehaviour
{
    public static PlayerInputReader instance;

    private InputSystem actions;

    public event Action OnJumpPressed;
    public event Action OnSkillMenuPressed;
    public event Action OnDashPressed;
    public event Action OnStealthPressed;
    public event Action OnEscPressed;
    public event Action OnDigPressed;

    public Vector2 Move { get; private set; }
    public bool SprintHeld { get; private set; }
    public bool SniffHeld { get; private set; }
    public bool DigHeld { get; private set; }
    public Vector2 DigDirection { get; private set; }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        actions = new InputSystem();
    }

    void OnEnable()
    {
        actions.Player.Move.performed += OnMove;
        actions.Player.Move.canceled += OnMove;
        actions.Player.Jump.performed += OnJump;
        actions.Player.Dash.performed += OnDash;
        actions.Player.Dig.performed += OnDig;
        actions.Player.Dig.canceled += OnDig;
        actions.Player.DigDirection.performed += OnDigDirection;
        actions.Player.DigDirection.canceled += OnDigDirection;
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
        actions.Player.Dig.performed -= OnDig;
        actions.Player.Dig.canceled -= OnDig;
        actions.Player.DigDirection.performed -= OnDigDirection;
        actions.Player.DigDirection.canceled -= OnDigDirection;
        actions.Player.Sprint.performed -= OnSprint;
        actions.Player.Sprint.canceled -= OnSprint;
        actions.Player.Sniff.performed -= OnSniff;
        actions.Player.Sniff.canceled -= OnSniff;
        actions.Player.Stealth.performed -= OnStealth;
        actions.Player.SkillMenu.performed -= OnSkillMenu;
        actions.Player.Pause.performed -= OnPause;

        actions.Disable();
    }

    public void EnableAction(InputActionReference actionRef)
    {
        var action = actions.asset.FindAction(actionRef.action.id);
        action?.Enable();
    }
    public void DisableAction(InputActionReference actionRef)
    {
        var action = actions.asset.FindAction(actionRef.action.id);
        action?.Disable();
    }
    private void OnMove(InputAction.CallbackContext ctx) => Move = ctx.ReadValue<Vector2>();
    private void OnJump(InputAction.CallbackContext ctx) => OnJumpPressed?.Invoke();
    private void OnDash(InputAction.CallbackContext ctx) => OnDashPressed?.Invoke();
    private void OnDig(InputAction.CallbackContext ctx)
    {
        DigHeld = ctx.performed;
        if (ctx.performed) OnDigPressed?.Invoke();
    }
    private void OnDigDirection(InputAction.CallbackContext ctx) => DigDirection = ctx.ReadValue<Vector2>();
    private void OnSprint(InputAction.CallbackContext ctx) => SprintHeld = ctx.performed;
    private void OnSniff(InputAction.CallbackContext ctx) => SniffHeld = ctx.performed;
    private void OnStealth(InputAction.CallbackContext ctx) => OnStealthPressed?.Invoke();
    private void OnSkillMenu(InputAction.CallbackContext ctx) => OnSkillMenuPressed?.Invoke();
    private void OnPause(InputAction.CallbackContext ctx) => OnEscPressed?.Invoke();
}