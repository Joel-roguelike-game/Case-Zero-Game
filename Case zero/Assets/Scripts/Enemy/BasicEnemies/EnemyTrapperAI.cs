using UnityEngine;

/*
 * EnemyTrapperAI
 *
 * Enemigo que persigue al jugador dejando charcos dañinos
 * de forma periódica mientras se mueve.
 */
public class EnemyTrapperAI : MonoBehaviour
{
    public GameObject puddlePrefab;
    public float puddleInterval = 1f;

    private EnemyStats stats;
    private Transform player;
    private float nextPuddleTime;

    /*
     * Inicializa referencias al jugador y a las estadísticas del enemigo.
     */
    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    /*
     * Persigue constantemente al jugador y genera charcos
     * de daño en intervalos regulares.
     */
    private void Update()
    {
        if (player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * stats.moveSpeed * Time.deltaTime);

        if (Time.time >= nextPuddleTime)
        {
            GameObject p = Instantiate(puddlePrefab, transform.position, Quaternion.identity);

            ToxicPuddle tp = p.GetComponent<ToxicPuddle>();
            if (tp != null)
            {
                tp.tickDamage = stats.baseDamage;
                tp.Init(GetComponent<EnemyCombat>());
            }
            
            nextPuddleTime = Time.time + puddleInterval;
        }
    }
}