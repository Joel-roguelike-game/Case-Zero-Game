using UnityEngine;

/*
 * EnemyMovement
 *
 * Comportamiento básico de movimiento para enemigos tipo Bruiser.
 * El enemigo persigue constantemente al jugador sin ataques especiales,
 * moviéndose directamente hacia su posición.
 */
public class EnemyMovement : MonoBehaviour
{
    private EnemyStats stats;
    private Transform player;

    /*
     * Obtiene las referencias necesarias:
     * - Estadísticas del enemigo (velocidad de movimiento).
     * - Transform del jugador.
     */
    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    /*
     * Movimiento continuo hacia el jugador.
     * Se ejecuta en FixedUpdate para mantener coherencia con la física.
     */
    private void FixedUpdate()
    {
        if (player == null)
            return;

        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * stats.moveSpeed * Time.fixedDeltaTime);
    }
}