using UnityEngine;
using System.Collections;

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
    public float dashMultiplier = 3f;  // Cuántas veces más rápido que la velocidad normal
    public float dashDuration = 0.15f; // duración del dash en segundos
    public float dashCooldown = 1f;

    private Vector2 minBounds;
    private Vector2 maxBounds;
    private bool canDash = true;
    private bool isDashing = false;
    private Vector2 dashDirection;

    private WeaponHandler weaponHandler;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputController>();
        playerCollider = GetComponent<Collider2D>();
        stats = GetComponent<PlayerStats>(); 
        weaponHandler = GetComponent<WeaponHandler>(); // lógica de armas
    }


    private void Start()
    {
        if (roomCollider != null)
        {
            minBounds = roomCollider.bounds.min;
            maxBounds = roomCollider.bounds.max;
        }

        // Suscribirse a eventos de input
        input.inputActions.Gameplay.Dodge.performed += ctx => OnDodge();
        input.inputActions.Gameplay.Parry.performed += ctx => OnParry();
        input.inputActions.Gameplay.MeleeAttack.performed += ctx => OnMelee();
        input.inputActions.Gameplay.RangedAttack.performed += ctx => OnRanged();
        input.inputActions.Ui.InGameMenu.performed += ctx => OnMenu();
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            MovePlayer();
        }
        ClampToRoomBounds();
    }

    // Mueve al jugador usando MovePosition
    private void MovePlayer()
    {
        rb.linearVelocity = input.MoveInput * stats.moveSpeed.Current;
    }

    // Impide que el jugador salga de los límites de la sala
    private void ClampToRoomBounds()
    {
        if (roomCollider == null) return;

        float halfW = playerCollider.bounds.extents.x;
        float halfH = playerCollider.bounds.extents.y;

        float clampedX = Mathf.Clamp(rb.position.x, minBounds.x + halfW, maxBounds.x - halfW);
        float clampedY = Mathf.Clamp(rb.position.y, minBounds.y + halfH, maxBounds.y - halfH);

        rb.position = new Vector2(clampedX, clampedY);
    }

    // MÉTODOS DE ACCIÓN
    private void OnDodge()
    {
        if (canDash && !isDashing && input.MoveInput != Vector2.zero)
        {
            Debug.Log("DODGED!");
            dashDirection = input.MoveInput.normalized;
            StartCoroutine(DashRoutine());
        }
    }

    private IEnumerator DashRoutine()
    {
        canDash = false;
        isDashing = true;

        // Preparar invulnerabilidad aquí (hook)
        // invulnerable = true;
        
        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            // Durante el dash, ignoramos input normal
            rb.linearVelocity = dashMultiplier * stats.moveSpeed.Current * dashDirection;
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // Fin del dash: restauramos velocidad normal
        isDashing = false;
        rb.linearVelocity = Vector2.zero;

        // Cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void OnParry() => Debug.Log("PARRY!");
    private void OnMelee() => weaponHandler.UseMelee();
    private void OnRanged() => weaponHandler.UseRanged();
    private void OnMenu() => Debug.Log("MENU!");
}

