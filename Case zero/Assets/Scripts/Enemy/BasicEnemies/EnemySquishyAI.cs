using UnityEngine;
using System.Collections;

/*
 * EnemySquishyAI
 * 
 * Enemigo frágil que persigue al jugador y ejecuta
 * un dash rápido cuando entra en rango.
 * Tras golpear, entra en cooldown antes de volver a atacar.
 */
public class EnemySquishyAI : MonoBehaviour
{
    public float engageDistance = 4f;
    public float chargeTime = 0.3f;
    public float dashSpeedMultiplier = 3f;
    public float postHitCooldown = 2f;

    private Transform player;
    private EnemyStats stats;
    private bool attacking;
    private bool hasHitPlayer;
    
    private Rigidbody2D rb;
    private Coroutine dashRoutine;

    /*
     * Inicializa referencias al jugador, estadísticas y Rigidbody.
     */
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        stats = GetComponent<EnemyStats>();
        rb = GetComponent<Rigidbody2D>();
    }

    /*
     * Controla el estado general del enemigo:
     * - Persigue al jugador si está lejos.
     * - Inicia el dash si entra en rango.
     * - Se bloquea temporalmente tras golpear al jugador.
     */
    private void Update()
    {
        if (attacking || player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (hasHitPlayer)
            return;

        if (dist > engageDistance)
        {
            MoveTowardsPlayer();
        }
        else if (!attacking)
        {
            dashRoutine = StartCoroutine(DashAttack());
        }
    }

    /*
     * Movimiento básico de persecución.
     */
    private void MoveTowardsPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * stats.moveSpeed * Time.deltaTime);
    }

    /*
     * Ataque de dash:
     * - Espera un breve tiempo de carga.
     * - Se lanza hacia el jugador hasta golpearlo.
     */
    private IEnumerator DashAttack()
    {
        attacking = true;
        yield return new WaitForSeconds(chargeTime);

        while (!hasHitPlayer)
        {
            if (player == null) break;

            Vector2 dir = (player.position - transform.position).normalized;
            rb.linearVelocity = dir * stats.moveSpeed * dashSpeedMultiplier;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
    }

    /*
     * Detecta el impacto con el jugador:
     * - Aplica daño.
     * - Detiene el dash.
     * - Inicia el cooldown post-impacto.
     */
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasHitPlayer)
        {
            hasHitPlayer = true;

            other.GetComponent<PlayerHealth>()
                ?.TakeDamage(stats.baseDamage, null);

            if (dashRoutine != null)
                StopCoroutine(dashRoutine);

            rb.linearVelocity = Vector2.zero;
            StartCoroutine(PostHitCooldown());
        }
    }

    /*
     * Cooldown tras impactar al jugador
     * antes de permitir un nuevo ataque.
     */
    private IEnumerator PostHitCooldown()
    {
        yield return new WaitForSeconds(postHitCooldown);
        hasHitPlayer = false;
        attacking = false;
    }
}
