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

    private float hpRegenTimer;
    private float timeSinceLastHit;
    
    [Header("Invulnerability")]
    public float invulDuration = 1f;
    public float blinkInterval = 0.1f;

    private bool invulnerable;
    private bool parryInvulActive;
    private bool dashInvulActive;
    
    [Header("Lifesteal")]
    public float lifestealCooldown = 1f;
    private float lastLifestealTime = -999f;


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
     * Controla la regeneración de vida.
     */
    private void Update()
    {
        timeSinceLastHit += Time.deltaTime;

        if (timeSinceLastHit < 1f || invulnerable)
            return;

        hpRegenTimer += Time.deltaTime;

        if (hpRegenTimer >= 1f)
        {
            stats.currentHp += stats.hpRegen.Current;
            stats.currentHp = Mathf.Min(
                stats.currentHp,
                stats.maxHP.Current
            );

            hpRegenTimer = 0f;
        }
    }
    /*
     * Aplica daño al jugador.
     * Si está invulnerable, ignora el daño y gestiona el parry.
     */
    public void TakeDamage(float dmg, EnemyCombat source = null)
    {
        if (invulnerable)
        {
            //Debug.Log("DAÑO IGNORADO");

            if (parryInvulActive && source != null)
            {
                source.SetParryAffected();
                SlowMotionController.Instance.TriggerParrySlow();
            }

            return;
        }

        stats.currentHp -= dmg;
        stats.currentHp = Mathf.Max(stats.currentHp, 0f);
        timeSinceLastHit = 0f;
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
    
    /*
     * Aplica curación por lifesteal al jugador si el cooldown lo permite.
     */
    public void TryApplyLifesteal()
    {
        if (Time.time < lastLifestealTime + lifestealCooldown)
            return;

        lastLifestealTime = Time.time;

        float percentHeal = stats.maxHP.Current * (stats.lifestealPercent.Current / 100f);

        float flatHeal = stats.lifestealFlat.Current;

        float totalHeal = percentHeal + flatHeal;

        Heal(totalHeal);
    }

    
    /*
     * Cura al jugador sin superar la vida máxima.
     */
    public void Heal(float amount)
    {
        stats.currentHp =
            Mathf.Min(stats.currentHp + amount, stats.maxHP.Current);
    }

}
