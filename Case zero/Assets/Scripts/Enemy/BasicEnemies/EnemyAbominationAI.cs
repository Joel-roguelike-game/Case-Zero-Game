using UnityEngine;
using System.Collections;

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

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        stats = GetComponent<EnemyStats>();
    }

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

    private void MoveTowardsPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * stats.moveSpeed * Time.deltaTime);
    }

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

    private IEnumerator DashAttack()
    {
        Vector2 targetPos = player.position;
        yield return new WaitForSeconds(chargeTime);

        Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
        float t = 0.3f;

        while (t > 0f)
        {
            transform.position += (Vector3)(dir * stats.moveSpeed * dashMultiplier * Time.deltaTime);
            t -= Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator AoeAttack()
    {
        yield return new WaitForSeconds(chargeTime);

        EnemyCombat combat = GetComponent<EnemyCombat>();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, aoeRadius);
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
