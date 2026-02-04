using UnityEngine;
using System.Collections;

/*
 * EnemyHealth
 *
 * Controla la vida del enemigo:
 * - Recepción de daño
 * - Muerte
 * - Desactivación de componentes
 * - Fade visual antes de destruirse
 */
public class EnemyHealth : MonoBehaviour
{
    private EnemyStats stats;
    private SpriteRenderer sr;

    // Indica si el enemigo ya está muerto (estado global)
    public bool isDead { get; private set; }

    /*
     * Inicializa referencias a estadísticas y SpriteRenderer.
     */
    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        sr = GetComponent<SpriteRenderer>();
    }

    /*
     * Aplica daño al enemigo y gestiona la muerte.
     */
    public void TakeDamage(DamageContext ctx)
    {
        if (isDead) return;

        stats.currentHealth -= ctx.damage;

        if (stats.currentHealth <= 0f)
            HandleDeath();
    }


    /*
     * Maneja la muerte del enemigo.
     * 
     * NUEVO:
     * - Marca el enemigo como muerto
     * - Detiene TODAS las coroutines
     * - Desactiva todos los scripts de comportamiento
     * - Detiene el Rigidbody
     * - Desactiva todos los colliders
     */
    private void HandleDeath()
    {
        isDead = true;

        // Detener TODAS las coroutines del enemigo
        StopAllCoroutines();

        // Desactivar todos los scripts excepto este
        foreach (var mb in GetComponents<MonoBehaviour>())
        {
            if (mb != this)
                mb.enabled = false;
        }

        // Detener cualquier movimiento físico
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // Desactivar todos los colliders (hitbox, triggers, etc.)
        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        // Iniciar la secuencia visual de muerte
        StartCoroutine(Die());
    }

    /*
     * Secuencia de muerte:
     * - Hace fade del sprite
     * - Destruye el GameObject
     */
    private IEnumerator Die()
    {
        float t = 0f;
        Color c = sr.color;

        while (t < 3f)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / 3f);
            sr.color = c;
            yield return null;
        }

        Destroy(gameObject);
    }
}
