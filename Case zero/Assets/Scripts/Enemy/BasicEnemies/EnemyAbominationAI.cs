using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

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
    //Ayuda a saber cuando parar el dash
    private bool isDashing;
    private Vector2 dashDirection;
    private Rigidbody2D rb;
    //indicador visual del ataque AoE
    public GameObject aoeIndicatorPrefab;
    private GameObject aoeIndicator;


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

        if (dist > engageDistance)
        {
            MoveTowardsPlayer();
        }
        else
        {
            StartCoroutine(RandomAttack());
        }
    }
    
    private void FixedUpdate()
    {
        if (isDashing)
            rb.linearVelocity = dashDirection * stats.moveSpeed * dashMultiplier;
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
        yield return new WaitForSeconds(chargeTime);

        dashDirection = (player.position - transform.position).normalized;
        isDashing = true;
    }


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

            Vector3 originalScale = aoeIndicator.transform.localScale;
            aoeIndicator.transform.localScale = Vector3.one;

            float spriteWorldDiameter = aoeSR.bounds.size.x;

            float desiredDiameter = aoeRadius * 2f;

            float scaleFactor = desiredDiameter / spriteWorldDiameter;

            aoeIndicator.transform.localScale = Vector3.one * scaleFactor;
            aoeSR.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1;
        }


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



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
    

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
