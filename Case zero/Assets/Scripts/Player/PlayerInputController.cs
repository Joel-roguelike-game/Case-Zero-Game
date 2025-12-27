using UnityEngine;
using UnityEngine.InputSystem;

/*
 * PlayerInputController
 * 
 * Centraliza y expone los inputs del jugador.
 * Usa el sistema de Input Actions de Unity.
 */
public class PlayerInputController : MonoBehaviour
{
    public PlayerInputActions inputActions;

    public Vector2 MoveInput { get; private set; }
    public bool DodgePressed { get; private set; }
    public bool ParryPressed { get; private set; }
    public bool MeleePressed { get; private set; }
    public bool RangedPressed { get; private set; }
    public bool MenuPressed { get; private set; }

    /*
     * Inicializa el sistema de input.
     */
    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    /*
     * Habilita los mapas de input y sus callbacks.
     */
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

        inputActions.Gameplay.Ability.performed += ctx => OnActive();

        // UI map
        inputActions.Ui.InGameMenu.performed += ctx => MenuPressed = true;
        inputActions.Ui.InGameMenu.canceled += ctx => MenuPressed = false;
    }

    /*
     * Deshabilita el sistema de input.
     */
    private void OnDisable()
    {
        inputActions.Disable();
    }

    /*
     * Actualiza el input de movimiento.
     */
    private void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }
    
    private void OnActive()
    {
        GetComponent<PlayerStats>().TryActivate();
    }

}
