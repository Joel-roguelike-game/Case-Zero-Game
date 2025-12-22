using UnityEngine;

/*
 * EnemyProjectile
 *
 * Proyectil disparado por enemigos ranged.
 * Se mueve en una dirección fija, aplica daño al jugador
 * y se destruye al impactar o al superar su tiempo de vida.
 */
public class EnemyProjectile : MonoBehaviour
{
    public float speed = 8f;
    public float lifeTime = 4f;

    private Vector2 direction;
    private float damage;
    private float timer;

    /*
     * Inicializa la dirección y el daño del proyectil.
     */
    public void Init(Vector2 dir, float dmg)
    {
        direction = dir.normalized;
        damage = dmg;
    }

    /*
     * Movimiento constante del proyectil y control de vida útil.
     */
    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= lifeTime)
            Destroy(gameObject);
    }

    /*
     * Detecta colisiones:
     * - Aplica daño al jugador
     * - Se destruye al tocar límites del mapa
     */
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()
                ?.TakeDamage(damage, null);
            Destroy(gameObject);
        }

        if (other.CompareTag("MapBoundary"))
            Destroy(gameObject);
    }
}