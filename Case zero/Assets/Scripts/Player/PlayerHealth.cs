using UnityEngine;
using System.Collections;
/*
 * Accede a la cida del jugador y gestiona el daño recibido
 * cotrola el parpadeo y la invulnerabilidad por daño del jugador.
 * controla la invulnerabilidad en parry y dash
 */
public class PlayerHealth : MonoBehaviour
{
    private PlayerStats stats;
    private SpriteRenderer sr;

    [Header("Invulnerability")]
    public float invulDuration = 1f;
    public float blinkInterval = 0.1f;

    private bool invulnerable;
    private bool parryInvulActive;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void TakeDamage(float dmg, EnemyCombat source = null)
    {
        if (invulnerable)
        {
            Debug.Log("DAÑO IGNORADO");

            if (parryInvulActive && source != null)
            {
                source.SetParryAffected();
            }

            return;
        }

        stats.currentHp -= dmg;
        stats.currentHp = Mathf.Max(stats.currentHp, 0f);

        StartCoroutine(Invulnerability(false));
    }


    

    private IEnumerator Invulnerability(bool isParry, float forcedDuration = -1f)
    {
        invulnerable = true;
        parryInvulActive = isParry;

        float duration = forcedDuration > 0f ? forcedDuration : invulDuration;

        float timer = 0f;
        while (timer < duration)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        sr.enabled = true;
        invulnerable = false;
        parryInvulActive = false;
    }
    
    public void StartParryInvulnerability(float duration)
    {
        StartCoroutine(Invulnerability(true, duration));
    }


}