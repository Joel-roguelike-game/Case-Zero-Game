using UnityEngine;

public class EnemyTrapperAI : MonoBehaviour
{
    public GameObject puddlePrefab;
    public float puddleInterval = 1f;

    private EnemyStats stats;
    private Transform player;
    private float nextPuddleTime;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

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
            }
            
            nextPuddleTime = Time.time + puddleInterval;
        }
    }
}