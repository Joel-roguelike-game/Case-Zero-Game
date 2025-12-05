using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    // Reference to the generated InputActions
    private PlayerInputActions inputActions;

    // Input variables exposed for other systems
    public Vector2 MoveInput { get; private set; }
    public bool DodgePressed { get; private set; }
    public bool ParryPressed { get; private set; }
    public bool MeleePressed { get; private set; }
    public bool RangedPressed { get; private set; }
    public bool MenuPressed { get; private set; }

    private void Awake()
    {
        // Instantiate and enable input actions
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        // Gameplay map
        inputActions.Gameplay.Move.performed += OnMove;
        inputActions.Gameplay.Move.canceled += OnMove;

        inputActions.Gameplay.Dodge.performed += ctx => DodgePressed = true;
        inputActions.Gameplay.Dodge.canceled += ctx => DodgePressed = false;

        inputActions.Gameplay.Parry.performed += ctx => ParryPressed = true;
        inputActions.Gameplay.Parry.canceled += ctx => ParryPressed = false;

        inputActions.Gameplay.MeleeAttack.performed += ctx => MeleePressed = true;
        inputActions.Gameplay.MeleeAttack.canceled += ctx => MeleePressed = false;

        inputActions.Gameplay.RangedAttack.performed += ctx => RangedPressed = true;
        inputActions.Gameplay.RangedAttack.canceled += ctx => RangedPressed = false;

        // UI map
        inputActions.Ui.InGameMenu.performed += ctx => MenuPressed = true;
        inputActions.Ui.InGameMenu.canceled += ctx => MenuPressed = false;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    // Move returns a Vector2 — this is the main difference
    private void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    // After update, reset one-frame buttons
    private void LateUpdate()
    {
        // All these are “one-frame” presses
        DodgePressed = false;
        ParryPressed = false;
        MeleePressed = false;
        RangedPressed = false;
        MenuPressed = false;
    }
}
