using UnityEngine;

/*
 El Bruiser persigue constantemente al jugador.
*/
public class EnemyMovement : MonoBehaviour
{
    private EnemyStats stats;
    private Transform player;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        if (player == null)
            return;

        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * stats.moveSpeed * Time.fixedDeltaTime);
    }
}