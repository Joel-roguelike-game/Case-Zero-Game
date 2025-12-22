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
    // Distancia a partir de la cual deja de moverse y empieza a atacar
    public float engageDistance = 6f;

    // Tiempo de carga previo a los ataques
    public float chargeTime = 0.5f;

    // Multiplicador de velocidad durante el dash
    public float dashMultiplier = 2.5f;

    // Radio del ataque en área
    public float aoeRadius = 2.5f;

    // Tiempo de espera entre ataques
    public float attackCooldown = 2f;

    // Referencia al jugador
    private Transform player;

    // Estadísticas del enemigo (vida, daño, velocidad, etc.)
    private EnemyStats stats;

    // Indica si el enemigo está realizando un ataque
    private bool attacking;

    // Ayuda a saber cuándo el enemigo está en una embestida activa
    private bool isDashing;

    // Dirección en la que se realiza el dash
    private Vector2 dashDirection;

    // Rigidbody para mover al enemigo durante el dash
    private Rigidbody2D rb;

    // Prefab visual que indica el área del ataque AOE
    public GameObject aoeIndicatorPrefab;

    // Instancia actual del indicador AOE
    private GameObject aoeIndicator;

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
        Debug.Log("AOE INDICATOR SPAWNED");
        
        SpriteRenderer aoeSR = aoeIndicator.GetComponent<SpriteRenderer>();
        if (aoeSR != null)
        {
            // Forzar escala base para calcular tamaño real del sprite
            Vector3 originalScale = aoeIndicator.transform.localScale;
            aoeIndicator.transform.localScale = Vector3.one;

            // Diámetro real del sprite en el mundo
            float spriteWorldDiameter = aoeSR.bounds.size.x;

            // Diámetro deseado según el radio del AOE
            float desiredDiameter = aoeRadius * 2f;

            // Factor de escala necesario para igualar el radio de daño
            float scaleFactor = desiredDiameter / spriteWorldDiameter;

            // Aplicar escala final
            aoeIndicator.transform.localScale = Vector3.one * scaleFactor;

            // Asegurar que el indicador se dibuje por debajo del enemigo
            aoeSR.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1;
        }

        // Fase de carga del ataque con parpadeo visual
        while (t < chargeTime)
        {
            // Mantener el AOE centrado en el enemigo
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
            Debug.Log("HA PARADO EMBESTIDA");
            isDashing = false;
            rb.linearVelocity = Vector2.zero;
        }
    }
}
