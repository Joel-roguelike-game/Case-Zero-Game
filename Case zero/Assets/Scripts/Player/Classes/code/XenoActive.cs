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
        // Se ejecuta inmediatamente, sin cooldown de tiempo
        stats.StartCoroutine(ApplyActive(stats));
    }

    /*
     * Rutina principal de la activa
     */
    private IEnumerator ApplyActive(PlayerStats stats)
    {
        PlayerHealth health = stats.GetComponent<PlayerHealth>();

        float missingHp = stats.maxHP.Current - stats.currentHp;
        float shieldAmount = missingHp * missingHpPercent;
        float hpBeforeShield = stats.currentHp;

        // Escudo como curación temporal
        health.Heal(shieldAmount);

        bool shieldBroken = false;
        float timer = 0f;

        while (timer < duration)
        {
            if (stats.currentHp < hpBeforeShield)
            {
                shieldBroken = true;

                float heal = stats.currentHp * healOnBreakPercent;
                health.Heal(heal);
                break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Si el escudo aguanta
        if (!shieldBroken)
        {
            float bonusMaxHp = stats.maxHP.Base * bonusMaxHpPercent;

            // Añadimos flat bonus a maxHP usando AddFlat
            stats.maxHP.AddFlat(bonusMaxHp);
            health.Heal(bonusMaxHp);

            yield return new WaitForSeconds(duration);

            stats.maxHP.AddFlat(-bonusMaxHp);

            // Ajustamos currentHp si supera el máximo
            stats.currentHp = Mathf.Min(stats.currentHp, stats.maxHP.Current);
        }
    }
}
