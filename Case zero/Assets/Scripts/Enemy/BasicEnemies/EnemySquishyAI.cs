using UnityEngine;
using System.Collections;

/*
 * EnemySquishyAI
 * 
 * Enemigo frágil que persigue al jugador y ejecuta
 * un dash rectilíneo hacia la última posición conocida del jugador.
 * El dash termina al golpear al jugador o al chocar con el mapa.
 */
public class EnemySquishyAI : MonoBehaviour
{
    public float engageDistance = 4f;
    public float chargeTime = 0.3f;
    public float dashSpeedMultiplier = 3f;
    public float attackCooldown = 2f;

    private Transform player;
    private EnemyStats stats;
    private EnemyCombat source;

    private Rigidbody2D rb;

    private bool attacking;
    private bool isDashing;
    private Vector2 dashDirection;

    /*
     * Inicializa referencias.
     */
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        stats = GetComponent<EnemyStats>();
        source = GetComponent<EnemyCombat>();
        rb = GetComponent<Rigidbody2D>();
    }

    /*
     * Lógica general:
     * - Persigue al jugador si está lejos
     * - Si entra en rango, inicia el ataque de dash
     */
    private void Update()
    {
        if (player == null || attacking)
            return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist > engageDistance)
        {
            MoveTowardsPlayer();
        }
        else
        {
            StartCoroutine(DashAttack());
        }
    }

    /*
     * Movimiento básico hacia el jugador.
     */
    private void MoveTowardsPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * stats.moveSpeed * Time.deltaTime);
    }

    /*
     * Movimiento físico del dash.
     */
    private void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = dashDirection * stats.moveSpeed * dashSpeedMultiplier;
        }
    }

    /*
     * Ataque de dash:
     * - Carga
     * - Calcula dirección fija
     * - Activa el dash
     */
    private IEnumerator DashAttack()
    {
        attacking = true;

        yield return new WaitForSeconds(chargeTime);

        if (player == null)
        {
            attacking = false;
            yield break;
        }

        dashDirection = (player.position - transform.position).normalized;
        isDashing = true;
    }

    /*
     * Gestión de colisiones durante el dash.
     */
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isDashing)
            return;

        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()
                ?.TakeDamage(stats.baseDamage, source);

            EndDash();
        }

        if (other.CompareTag("MapBoundary"))
        {
            EndDash();
        }
    }

    /*
     * Finaliza el dash y entra en cooldown.
     */
    private void EndDash()
    {
        isDashing = false;
        rb.linearVelocity = Vector2.zero;
        StartCoroutine(AttackCooldown());
    }

    /*
     * Cooldown tras el ataque.
     */
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        attacking = false;
    }
}
