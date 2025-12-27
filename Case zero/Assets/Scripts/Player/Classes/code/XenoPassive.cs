using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Classes/Passives/XenoPassive")]
public class XenoPassive : SOClassPassive
{
    public float maxHpPercentBonus = 0.01f; // 1% de la vida máxima
    public float currentHpPercentBonus = 0.02f; // 2% de la vida restante

    private PlayerStats stats;
    private bool active = false;

    public override void Activate(PlayerStats stats)
    {
        this.stats = stats;
        if (!active)
        {
            active = true;
            stats.StartCoroutine(UpdateDamageBonus());
        }
    }

    public override void Deactivate(PlayerStats stats)
    {
        active = false;
    }

    private IEnumerator UpdateDamageBonus()
    {
        while (active)
        {
            float bonusFromMax = stats.maxHP.Current * maxHpPercentBonus;
            float bonusFromCurrent = (stats.maxHP.Current-stats.currentHp) * currentHpPercentBonus;

            float totalBonus = bonusFromMax + bonusFromCurrent;

            stats.caCDmg.Current = stats.pClass.caCDmg + totalBonus;
            stats.distDmg.Current = stats.pClass.distDmg + totalBonus;

            yield return null; // recalcula cada frame
        }
    }
}