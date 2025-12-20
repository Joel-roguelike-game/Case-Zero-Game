using UnityEngine;

/*
 Enemigo ranged básico:
 - Se acerca hasta una distancia mínima
 - Dispara proyectiles con cooldown
*/
public class EnemyShooterAI : MonoBehaviour
{
    public float stopDistance = 5f;
    public float shootCooldown = 1f;
    public GameObject projectilePrefab;

    private Transform player;
    private EnemyStats stats;
    private float nextShootTime;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        stats = GetComponent<EnemyStats>();
    }

    private void Update()
    {
        if (player == null || stats == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist > stopDistance)
            MoveTowardsPlayer();
        else
            TryShoot();
    }

    private void MoveTowardsPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * stats.moveSpeed * Time.deltaTime);
    }

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
        ep.Init(dir, stats.baseDamage);

        nextShootTime = Time.time + shootCooldown;
    }
}