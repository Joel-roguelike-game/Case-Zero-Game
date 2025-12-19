using UnityEngine;
using System.Collections;

public class EnemySquishyAI : MonoBehaviour
{
    public float engageDistance = 4f;
    public float chargeTime = 0.3f;
    public float dashSpeedMultiplier = 3f;

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
            StartCoroutine(DashAttack());
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * stats.moveSpeed * Time.deltaTime);
    }

    private IEnumerator DashAttack()
    {
        attacking = true;
        yield return new WaitForSeconds(chargeTime);

        while (true)
        {
            if (player == null) break;

            Vector2 dir = (player.position - transform.position).normalized;
            transform.position += (Vector3)(dir * stats.moveSpeed * dashSpeedMultiplier * Time.deltaTime);
            yield return null;
        }
    }
}