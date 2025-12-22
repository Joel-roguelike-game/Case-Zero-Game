using UnityEngine;
using System.Collections;

/*
 * EnemyNecromancerAI
 * 
 * Enemigo invocador que mantiene la distancia del jugador.
 * Si el jugador está lejos, invoca enemigos aliados.
 * Si el jugador se acerca demasiado, deja de invocar y ataca cuerpo a cuerpo.
 */
public class EnemyNecromancerAI : MonoBehaviour
{
    public float summonTime = 3f;
    public float meleeRange = 2f;
    public GameObject bruiserPrefab;

    private Transform player;
    private EnemyStats stats;
    private bool summoning;

    /*
     * Inicializa referencias básicas:
     * - Transform del jugador.
     * - Estadísticas propias del enemigo.
     */
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        stats = GetComponent<EnemyStats>();
    }

    /*
     * Decide el comportamiento según la distancia al jugador:
     * - Si está lejos, intenta invocar enemigos.
     * - Si está cerca, cancela invocaciones y ataca en melee.
     */
    private void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist > meleeRange)
        {
            if (!summoning)
                StartCoroutine(SummonRoutine());
        }
        else
        {
            StopAllCoroutines();
            summoning = false;
            MeleeAttack();
        }
    }

    /*
     * Rutina de invocación:
     * - Espera un tiempo de canalización.
     * - Invoca varios enemigos aliados alrededor del necromancer.
     * - Aplica el nivel del necromancer a los enemigos invocados.
     */
    private IEnumerator SummonRoutine()
    {
        summoning = true;
        yield return new WaitForSeconds(summonTime);

        for (int i = 0; i < 2; i++)
        {
            GameObject b = Instantiate(bruiserPrefab, transform.position + Random.insideUnitSphere, Quaternion.identity);

            EnemyStats es = b.GetComponent<EnemyStats>();
            if (es != null)
            {
                es.InitializeFromLevel(stats.level);
            }
        }

        summoning = false;
    }

    /*
     * Ataque cuerpo a cuerpo en área:
     * - Detecta al jugador dentro del rango.
     * - Aplica daño usando el sistema de combate del enemigo.
     */
    private void MeleeAttack()
    {
        EnemyCombat combat = GetComponent<EnemyCombat>();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, meleeRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<PlayerHealth>()?
                    .TakeDamage(stats.baseDamage, combat);
            }
        }
    }
}
