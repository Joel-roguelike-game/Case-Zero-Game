using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

/*
 * EnemyAbominationAI
 * 
 * El Abomination es un enemigo pesado con dos tipos de ataque:
 * - Una embestida (dash) en dirección al jugador.
 * - Un ataque en área (AOE) que se carga y daña en un radio determinado.
 * 
 * Alterna entre ambos ataques cuando el jugador está a distancia de combate.
 */
public class EnemyAbominationAI : MonoBehaviour
{

    public float engageDistance = 6f;
    public float chargeTime = 0.5f;
    public float dashMultiplier = 2.5f;
    public float aoeRadius = 2.5f;
    public float attackCooldown = 2f;
    private Transform player;
    private EnemyStats stats;
    private bool attacking;
    private bool isDashing;
    private Vector2 dashDirection;
    private Rigidbody2D rb;
    public GameObject aoeIndicatorPrefab;
    private GameObject aoeIndicator;
    private EnemyHealth health;

    /*
     * Inicializa referencias necesarias:
     * - Jugador
     * - Stats del enemigo
     * - Rigidbody2D
     */
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        stats = GetComponent<EnemyStats>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<EnemyHealth>();

    }

    /*
     * Lógica principal del enemigo.
     * 
     * Si está atacando o el jugador no existe, no hace nada.
     * Si el jugador está lejos, se mueve hacia él.
     * Si está a distancia de combate, inicia un ataque aleatorio.
     */
    private void Update()
    {
        if (attacking || player == null) return;

        if (health != null && health.isDead)
            return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist > engageDistance)
        {
            MoveTowardsPlayer();
        }
        else
        {
            StartCoroutine(RandomAttack());
        }
    }
    
    /*
     * Movimiento físico del dash.
     * 
     * Mientras el enemigo esté embistiendo, se le asigna una velocidad
     * constante en la dirección calculada previamente.
     */
    private void FixedUpdate()
    {
        if (health != null && health.isDead)
            return;

        if (isDashing)
            rb.linearVelocity = dashDirection * stats.moveSpeed * dashMultiplier;
    }

    /*
     * Movimiento básico hacia el jugador cuando no está atacando.
     */
    private void MoveTowardsPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * stats.moveSpeed * Time.deltaTime);
    }

    /*
     * Selecciona un ataque de forma aleatoria:
     * - DashAttack
     * - AoeAttack
     * 
     * Tras ejecutar el ataque, espera el cooldown antes de permitir otro.
     */
    private IEnumerator RandomAttack()
    {
        attacking = true;

        if (Random.value < 0.5f)
            yield return DashAttack();
        else
            yield return AoeAttack();

        yield return new WaitForSeconds(attackCooldown);
        attacking = false;
    }

    /*
     * Ataque de embestida.
     * 
     * Espera el tiempo de carga y luego activa el dash
     * en dirección al jugador.
     */
    private IEnumerator DashAttack()
    {
        yield return new WaitForSeconds(chargeTime);

        dashDirection = (player.position - transform.position).normalized;
        isDashing = true;
    }

    /*
     * Ataque en área.
     * 
     * Muestra un indicador visual del radio del ataque mientras se carga,
     * parpadea al enemigo para dar feedback visual y, al finalizar la carga,
     * aplica daño a todos los jugadores dentro del radio.
     */
    private IEnumerator AoeAttack()
    {
        float t = 0f;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color baseColor = sr.color;

        aoeIndicator = Instantiate(
            aoeIndicatorPrefab,
            transform.position,
            Quaternion.identity
        );
        //Debug.Log("AOE INDICATOR SPAWNED");
        
        SpriteRenderer aoeSR = aoeIndicator.GetComponent<SpriteRenderer>();
        if (aoeSR != null)
        {

            Vector3 originalScale = aoeIndicator.transform.localScale;
            aoeIndicator.transform.localScale = Vector3.one;

            float spriteWorldDiameter = aoeSR.bounds.size.x;

            float desiredDiameter = aoeRadius * 2f;

            float scaleFactor = desiredDiameter / spriteWorldDiameter;

            aoeIndicator.transform.localScale = Vector3.one * scaleFactor;

            aoeSR.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1;
        }

        // Fase de carga del ataque con parpadeo visual
        while (t < chargeTime)
        {
            if (health != null && health.isDead)
                yield break;
            
            if (aoeIndicator == null)
                yield break;

            aoeIndicator.transform.position = transform.position;

            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);

            sr.color = baseColor;
            yield return new WaitForSeconds(0.1f);

            t += 0.2f;
        }


        sr.color = baseColor;
        Destroy(aoeIndicator);

        // Aplicar daño a todos los jugadores dentro del radio
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            aoeRadius
        );

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<PlayerHealth>()
                    ?.TakeDamage(stats.baseDamage, null);
            }
        }
    }

    /*
     * Limpieza forzada al desactivar el enemigo.
     * - Elimina el indicador AOE si estaba activo
     * - Cancela cualquier embestida en curso
     */
    private void OnDisable()
    {
        if (aoeIndicator != null)
        {
            Destroy(aoeIndicator);
            aoeIndicator = null;
        }

        isDashing = false;
    }


    
    /*
     * Indicador visual del radio del AOE en el editor.
     * Útil para debug y balanceo. Borrable más adelante.
     */
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
    
    /*
     * Detiene la embestida cuando el enemigo colisiona
     * con los límites del mapa.
     */
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MapBoundary") && isDashing)
        {
            //Debug.Log("HA PARADO EMBESTIDA");
            isDashing = false;
            rb.linearVelocity = Vector2.zero;
        }
    }
}
