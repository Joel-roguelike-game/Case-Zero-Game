using System.Collections;
using UnityEngine;

/*
 * XenoActive
 *
 * Activa: El Imperecedero
 * - Estado durante 10s
 * - +30% vida máxima
 * - Cura 4% de vida máxima por segundo
 * - +50% rango CaC
 * - Cada golpe recibido:
 *      +5% vida máxima (max 6 stacks = 30%)
 * - Al terminar:
 *      La vida máxima extra se mantiene 8s más
 * - No se puede reactivar si ya está activa
 */
[CreateAssetMenu(menuName = "Classes/Actives/XenoActive")]
public class XenoActive : SOClassActive
{
    [Header("Base Effects")]
    public float duration = 10f;
    public float bonusMaxHpPercent = 0.30f;
    public float healPerSecondPercent = 0.04f;
    public float meleeRangeBonus = 0.50f;

    [Header("On Hit Scaling")]
    public float bonusMaxHpPerHit = 0.05f;
    public int maxStacks = 6;

    [Header("After Effect")]
    public float postDuration = 8f;

    private bool isRunning;

    public override void Activar(PlayerStats stats)
    {
        Debug.Log($"[XenoActive] Try activate | Focus:{stats.currentFocus}");

        if (isRunning)
        {
            Debug.Log("[XenoActive] ❌ Ya está activa");
            return;
        }

        if (!stats.ConsumeFocus(focusCost))
        {
            Debug.Log("[XenoActive] ❌ Focus insuficiente");
            return;
        }

        stats.StartCoroutine(Run(stats));
    }

    private IEnumerator Run(PlayerStats stats)
    {
        isRunning = true;
        Debug.Log("[XenoActive] IMPERECEDERO START");

        float baseMaxHpBonus =
            stats.maxHP.Base * bonusMaxHpPercent;

        stats.maxHP.AddFlat(baseMaxHpBonus);
        stats.caCRange.AddFlat(stats.caCRange.Base * meleeRangeBonus);

        float totalExtraHp = baseMaxHpBonus;
        int stacks = 0;

        float previousHp = stats.currentHp;
        float end = Time.time + duration;

        while (Time.time < end)
        {
            // Curación por segundo
            float heal =
                stats.maxHP.Current * healPerSecondPercent * Time.deltaTime;

            stats.currentHp = Mathf.Min(
                stats.currentHp + heal,
                stats.maxHP.Current
            );

            // Detectar golpe recibido
            if (stats.currentHp < previousHp && stacks < maxStacks)
            {
                stacks++;
                float bonus =
                    stats.maxHP.Base * bonusMaxHpPerHit;

                stats.maxHP.AddFlat(bonus);
                totalExtraHp += bonus;

                Debug.Log($"[XenoActive] Hit stack {stacks}/{maxStacks} | +HP {bonus:F1}");
            }

            previousHp = stats.currentHp;
            yield return null;
        }

        // Quitamos buffs temporales visuales
        stats.caCRange.AddFlat(
            -(stats.caCRange.Base * meleeRangeBonus)
        );

        Debug.Log("[XenoActive] IMPERECEDERO END → Post duration");

        yield return new WaitForSeconds(postDuration);

        stats.maxHP.AddFlat(-totalExtraHp);
        stats.currentHp = Mathf.Min(
            stats.currentHp,
            stats.maxHP.Current
        );

        isRunning = false;
        Debug.Log("[XenoActive] IMPERECEDERO FULL END");
    }
}
