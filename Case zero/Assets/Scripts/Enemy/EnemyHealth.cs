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
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        stats.currentHealth -= amount;

        if (stats.currentHealth <= 0f)
        {
            isDead = true;
            StartCoroutine(Die());

            EnemyCombat ec = GetComponent<EnemyCombat>();
            if (ec != null) ec.enabled = false;

            EnemyMovement em = GetComponent<EnemyMovement>();
            if (em != null) em.enabled = false;

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            Collider2D[] cols = GetComponentsInChildren<Collider2D>();
            foreach (var col in cols)
                col.enabled = false;
        }
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