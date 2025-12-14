using UnityEngine;
using System.Collections;
/*
 * Accede a la cida del jugador y gestiona el daño recibido
 * cotrola el parpadeo y la invulnerabilidad por daño del jugador.
 */
public class PlayerHealth : MonoBehaviour
{
    private PlayerStats stats;
    private SpriteRenderer sr;

    [Header("Invulnerability")]
    public float invulDuration = 1f;
    public float blinkInterval = 0.1f;

    private bool invulnerable;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void TakeDamage(float dmg)
    {
        if (invulnerable)
            return;

        stats.currentHp -= dmg;
        stats.currentHp = Mathf.Max(stats.currentHp, 0f);

        StartCoroutine(Invulnerability());
    }

    private IEnumerator Invulnerability()
    {
        invulnerable = true;

        float timer = 0f;
        while (timer < invulDuration)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        sr.enabled = true;
        invulnerable = false;
    }
}