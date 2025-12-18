using UnityEngine;
using System.Collections;

/*
 Controla la vida y la muerte del enemigo.
 Al morir hace fade y se destruye.
*/
public class EnemyHealth : MonoBehaviour
{
    private EnemyStats stats;
    private SpriteRenderer sr;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float amount)
    {
        stats.currentHealth -= amount;

        if (stats.currentHealth <= 0f)
        {
            StartCoroutine(Die());
            GetComponent<EnemyCombat>().enabled = false;
            GetComponent<EnemyMovement>().enabled = false;
            
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            Collider2D[] cols = GetComponentsInChildren<Collider2D>();
            foreach (var col in cols)
                col.enabled = false;
        }
    }

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