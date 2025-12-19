using UnityEngine;
using System.Collections;

/*
 * Accede a la vida del jugador y gestiona el daño recibido
 * controla el parpadeo y la invulnerabilidad por daño del jugador.
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
    private bool dashInvulActive;

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
                SlowMotionController.Instance.TriggerParrySlow();
            }

            return;
        }

        stats.currentHp -= dmg;
        stats.currentHp = Mathf.Max(stats.currentHp, 0f);

        StartCoroutine(Invulnerability(false));
    }

    private IEnumerator Invulnerability(bool isParry, float forcedDuration = -1f, bool isDash = false)
    {
        invulnerable = true;
        parryInvulActive = isParry;
        dashInvulActive = isDash;

        float duration = forcedDuration > 0f ? forcedDuration : invulDuration;

        // === COLORES ===
        if (parryInvulActive)
            sr.color = Color.red;
        else if (dashInvulActive)
            sr.color = Color.yellow;

        float timer = 0f;

        // Parpadeo SOLO para daño normal
        if (!parryInvulActive && !dashInvulActive)
        {
            while (timer < duration)
            {
                sr.enabled = !sr.enabled;
                yield return new WaitForSeconds(blinkInterval);
                timer += blinkInterval;
            }
        }
        else
        {
            yield return new WaitForSeconds(duration);
        }

        // Reset
        sr.enabled = true;
        sr.color = Color.white;
        invulnerable = false;
        parryInvulActive = false;
        dashInvulActive = false;
    }

    public void StartParryInvulnerability(float duration)
    {
        StartCoroutine(Invulnerability(true, duration));
    }

    public void StartDashInvulnerability(float duration)
    {
        StartCoroutine(Invulnerability(false, duration, true));
    }
}
