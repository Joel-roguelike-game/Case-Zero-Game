using UnityEngine;
using System.Collections;

public class EnemyNecromancerAI : MonoBehaviour
{
    public float summonTime = 3f;
    public float meleeRange = 2f;
    public GameObject bruiserPrefab;

    private Transform player;
    private EnemyStats stats;
    private bool summoning;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        stats = GetComponent<EnemyStats>();
    }

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