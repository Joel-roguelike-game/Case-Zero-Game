using UnityEngine;

/*
 * EnemyShooterAI
 * 
 * Enemigo a distancia básico.
 * Se acerca al jugador hasta una distancia mínima
 * y dispara proyectiles de forma periódica usando cooldown.
 */
public class EnemyShooterAI : MonoBehaviour
{
    public float stopDistance = 5f;
    public float shootCooldown = 1f;
    public GameObject projectilePrefab;

    private Transform player;
    private EnemyStats stats;
    private float nextShootTime;
    private EnemyCombat source;

    /*
     * Inicializa referencias al jugador y a las estadísticas del enemigo.
     */
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        stats = GetComponent<EnemyStats>();
        source = GetComponent<EnemyCombat>();
    }

    /*
     * Decide el comportamiento según la distancia al jugador:
     * - Si está lejos, se mueve hacia él.
     * - Si está a rango, intenta disparar.
     */
    private void Update()
    {
        if (player == null || stats == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist > stopDistance)
            MoveTowardsPlayer();
        else
            TryShoot();
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
     * Gestiona el disparo del enemigo:
     * - Respeta el cooldown.
     * - Instancia un proyectil y le asigna dirección y daño.
     */
    private void TryShoot()
    {
        if (Time.time < nextShootTime) return;

        Vector2 dir = (player.position - transform.position).normalized;

        GameObject proj = Instantiate(
            projectilePrefab,
            transform.position,
            Quaternion.identity
        );

        EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
        ep.Init(dir, stats.baseDamage, source);

        nextShootTime = Time.time + shootCooldown;
    }
}
