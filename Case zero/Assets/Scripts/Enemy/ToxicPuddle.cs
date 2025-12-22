using UnityEngine;

/*
 * ToxicPuddle
 *
 * Zona de daño persistente creada por el Trapper.
 * Aplica daño continuo al jugador y se destruye tras un tiempo.
 */
public class ToxicPuddle : MonoBehaviour
{
    public float lifeTime = 5f;
    public float damagePerSecond = 5f;
    private float timer;

    /*
     * Controla la duración del charco.
     */
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifeTime)
            Destroy(gameObject);
    }

    public float tickDamage = 5f;
    public float tickInterval = 0.5f;

    /*
     * Aplica daño al jugador mientras permanece dentro del charco.
     */
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        other.GetComponent<PlayerHealth>()
            ?.TakeDamage(tickDamage, null);
    }
}