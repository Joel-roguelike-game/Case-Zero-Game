using System.Collections;
using UnityEngine;

/*
 * XenoPassive
 *
 * Pasiva: El Fortalecido
 * - El daño CaC y Dist escala con la vida máxima
 * - El porcentaje depende del % de vida actual
 *   100–66% → 10%
 *   66–33%  → 12.5%
 *   33–1%   → 15%
 * - Se recalcula dinámicamente
 */
[CreateAssetMenu(menuName = "Classes/Passives/XenoPassive")]
public class XenoPassive : SOClassPassive
{
    [Header("Scaling Percentages")]
    public float highHpPercent = 0.10f;
    public float midHpPercent = 0.125f;
    public float lowHpPercent = 0.15f;

    private Coroutine routine;
    private int lastTier = -1;

    public override void Activate(PlayerStats stats)
    {
        Debug.Log("[XenoPassive] ACTIVADA");
        routine = stats.StartCoroutine(UpdateBonus(stats));
    }

    public override void Deactivate(PlayerStats stats)
    {
        Debug.Log("[XenoPassive] DESACTIVADA");

        if (routine != null)
            stats.StopCoroutine(routine);

        stats.caCDmg.FlatBonus = 0f;
        stats.distDmg.FlatBonus = 0f;

        stats.caCDmg.Recalculate();
        stats.distDmg.Recalculate();
    }

    private IEnumerator UpdateBonus(PlayerStats stats)
    {
        while (true)
        {
            float hpPercent =
                stats.currentHp / stats.maxHP.Current;

            float scale;
            int tier;

            if (hpPercent > 0.66f)
            {
                scale = highHpPercent;
                tier = 0;
            }
            else if (hpPercent > 0.33f)
            {
                scale = midHpPercent;
                tier = 1;
            }
            else
            {
                scale = lowHpPercent;
                tier = 2;
            }

            float bonus = stats.maxHP.Current * scale;

            stats.caCDmg.FlatBonus = bonus;
            stats.distDmg.FlatBonus = bonus;

            stats.caCDmg.Recalculate();
            stats.distDmg.Recalculate();

            if (tier != lastTier)
            {
                Debug.Log(
                    $"[XenoPassive] Tier {tier} | HP% {(hpPercent * 100f):F1}% | Bonus {bonus:F1}"
                );
                lastTier = tier;
            }

            yield return null;
        }
    }
}
