using UnityEngine;
using System.Collections;

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

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        stats = GetComponent<EnemyStats>();
        rb = GetComponent<Rigidbody2D>();

    }

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

    private void MoveTowardsPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * stats.moveSpeed * Time.deltaTime);
    }

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


    private IEnumerator PostHitCooldown()
    {
        yield return new WaitForSeconds(postHitCooldown);
        hasHitPlayer = false;
        attacking = false;
    }


}