using System.Collections;
using UnityEngine;

/*
 * XenoActive
 * 
 * Activa: Fuerza imparable
 * - Otorga un escudo equivalente al 30% de la vida faltante durante 10s
 * - Si el escudo se rompe:
 *      • Cura instantáneamente un 25% de la vida actual
 * - Si el escudo aguanta:
 *      • Consume el escudo
 *      • Otorga +25% de vida máxima base durante 10s
 *      • Cura esa cantidad
 * - Cooldown: definido en SOClassActive
 */
[CreateAssetMenu(fileName = "XenoActive", menuName = "Classes/Actives/XenoActive")]
public class XenoActive : SOClassActive
{
    public float duration = 10f;
    public float missingHpPercent = 0.3f;
    public float healOnBreakPercent = 0.25f;
    public float bonusMaxHpPercent = 0.25f;

    public override void Activar(PlayerStats stats)
    {
        if (Time.time < stats.lastActiveTime + cooldown)
            return;

        stats.lastActiveTime = Time.time;
        stats.StartCoroutine(ApplyActive(stats));
    }

    /*
     * Rutina principal de la activa
     */
    private IEnumerator ApplyActive(PlayerStats stats)
    {
        PlayerHealth health = stats.GetComponent<PlayerHealth>();

        // Vida que falta
        float missingHp = stats.maxHP.Current - stats.currentHp;

        // Escudo como vida temporal
        float shieldAmount = missingHp * missingHpPercent;
        float hpBeforeShield = stats.currentHp;

        // Aplicamos el "escudo"
        health.Heal(shieldAmount);

        bool shieldBroken = false;
        float timer = 0f;

        while (timer < duration)
        {
            // Si la vida baja del valor previo al escudo → se rompió
            if (stats.currentHp < hpBeforeShield)
            {
                shieldBroken = true;

                // Cura inmediata al romperse
                float heal = stats.currentHp * healOnBreakPercent;
                health.Heal(heal);
                break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Si el escudo NO se rompió
        if (!shieldBroken)
        {
            float bonusMaxHp = stats.maxHP.Base * bonusMaxHpPercent;

            stats.maxHP.Current += bonusMaxHp;
            health.Heal(bonusMaxHp);

            yield return new WaitForSeconds(duration);

            // Retiramos la vida máxima temporal
            stats.maxHP.Current -= bonusMaxHp;
            stats.currentHp = Mathf.Min(stats.currentHp, stats.maxHP.Current);
        }
    }
}