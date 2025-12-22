using UnityEngine;
using System.Collections;

/*
 * PlayerHealth
 * 
 * Gestiona la vida del jugador y el daño recibido.
 * Controla:
 * - Invulnerabilidad tras recibir daño
 * - Parpadeo visual
 * - Invulnerabilidad especial durante parry y dash
 * - Interacción con el sistema de parry de los enemigos
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

    /*
     * Obtiene referencias a las estadísticas del jugador
     * y al SpriteRenderer para efectos visuales.
     */
    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    /*
     * Aplica daño al jugador.
     * Si está invulnerable, ignora el daño y gestiona el parry.
     */
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

    /*
     * Rutina de invulnerabilidad.
     * Puede activarse por daño normal, parry o dash.
     */
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

    /*
     * Activa la invulnerabilidad específica del parry.
     */
    public void StartParryInvulnerability(float duration)
    {
        StartCoroutine(Invulnerability(true, duration));
    }

    /*
     * Activa la invulnerabilidad específica del dash.
     */
    public void StartDashInvulnerability(float duration)
    {
        StartCoroutine(Invulnerability(false, duration, true));
    }
}
