using UnityEngine;
using System.Collections;

/*
 * PlayerMovement
 * 
 * Controla el movimiento del jugador:
 * - Movimiento básico
 * - Dash
 * - Parry
 * - Restricción a los límites de la sala
 * - Integración con el sistema de input y estadísticas
 */
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerInputController input;
    private Collider2D playerCollider;
    private PlayerStats stats;

    [Header("Room Bounds")]
    public BoxCollider2D roomCollider; // Asignar en Inspector

    [Header("Dash Settings")]
    public float dashMultiplier = 3f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;

    private Vector2 minBounds;
    private Vector2 maxBounds;
    private bool canDash = true;
    public bool isDashing = false;
    private Vector2 dashDirection;

    private WeaponHandler weaponHandler;

    public bool isParrying;
    private bool canParry = true;

    /*
     * Obtiene todas las referencias necesarias.
     */
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputController>();
        playerCollider = GetComponent<Collider2D>();
        stats = GetComponent<PlayerStats>(); 
        weaponHandler = GetComponent<WeaponHandler>();
    }

    /*
     * Controla la regeneración de stamina del jugador.
     */
    private void Update()
    {
        bool canRegenStamina =
            !weaponHandler.isAttacking &&
            !isDashing &&
            !isParrying;

        stats.RegenerateStamina(canRegenStamina);
    }
    /*
     * Inicializa límites de la sala y callbacks de input.
     */
    private void Start()
    {
        if (roomCollider != null)
        {
            minBounds = roomCollider.bounds.min;
            maxBounds = roomCollider.bounds.max;
        }

        input.inputActions.Gameplay.Dodge.performed += ctx => OnDodge();
        input.inputActions.Gameplay.Parry.performed += ctx => OnParry();
        input.inputActions.Ui.InGameMenu.performed += ctx => OnMenu();
    }

    /*
     * Controla el movimiento físico del jugador.
     */
    private void FixedUpdate()
    {
        if (!isDashing)
        {
            MovePlayer();
        }
        ClampToRoomBounds();
    }

    /*
     * Mueve al jugador usando velocidad del Rigidbody.
     */
    private void MovePlayer()
    {
        rb.linearVelocity = input.MoveInput * stats.moveSpeed.Current;
    }

    /*
     * Evita que el jugador salga de los límites de la sala.
     */
    private void ClampToRoomBounds()
    {
        if (roomCollider == null) return;

        float halfW = playerCollider.bounds.extents.x;
        float halfH = playerCollider.bounds.extents.y;

        float clampedX = Mathf.Clamp(rb.position.x, minBounds.x + halfW, maxBounds.x - halfW);
        float clampedY = Mathf.Clamp(rb.position.y, minBounds.y + halfH, maxBounds.y - halfH);

        rb.position = new Vector2(clampedX, clampedY);
    }

    /*
     * Inicia el dash si se cumplen las condiciones.
     */
    private void OnDodge()
    {
        if (!canDash || isDashing || isParrying || input.MoveInput == Vector2.zero)
            return;
        //intenta consumir stamina
        if (!stats.ConsumeStamina(20f))
            return;

        //Debug.Log("DODGED!");
        dashDirection = input.MoveInput.normalized;
        StartCoroutine(DashRoutine());
    }

    /*
     * Rutina completa del dash.
     */
    private IEnumerator DashRoutine()
    {
        canDash = false;
        isDashing = true;
        
        GetComponent<PlayerHealth>().StartDashInvulnerability(0.4f);

        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            rb.linearVelocity = dashMultiplier * stats.moveSpeed.Current * dashDirection;
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isDashing = false;
        rb.linearVelocity = Vector2.zero;

        float effectiveCooldown = dashCooldown / stats.dodgeSpeed.Current;
        yield return new WaitForSeconds(effectiveCooldown);
        canDash = true;

    }

    /*
     * Inicia el parry si está disponible.
     */
    private void OnParry()
    {
        if (!canParry || isDashing || isParrying)
            return;

        if (!stats.ConsumeStamina(30f))
            return;

        StartCoroutine(ParryRoutine());
    }

    /*
     * Rutina completa del parry.
     */
    private IEnumerator ParryRoutine()
    {
        Debug.Log("PARRY ACTIVADO");

        canParry = false;
        isParrying = true;

        // Aplicamos ralentización temporal
        stats.moveSpeed.AddMultiplier(0.2f);

        GetComponent<PlayerHealth>().StartParryInvulnerability(0.25f);

        yield return new WaitForSeconds(0.15f);

        // Restauramos velocidad
        stats.moveSpeed.Multiplier = 1f;
        stats.moveSpeed.Recalculate();

        isParrying = false;

        yield return new WaitForSeconds(2.85f);
        canParry = true;
    }

    /*
     * Callback del menú.
     */
    private void OnMenu() => Debug.Log("MENU!");
}
