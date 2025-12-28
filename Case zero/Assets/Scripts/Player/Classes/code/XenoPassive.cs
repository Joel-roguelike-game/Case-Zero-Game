using System.Collections;
using UnityEngine;

/*
 * XenoPassive
 *
 * Pasiva: Flujo de fuerza
 * - Aumenta el daño CaC y a distancia en:
 *   • 1% de la vida máxima actual
 *   • +2% de la vida que falte hasta la vida máxima
 * - Se recalcula constantemente para reflejar cambios en HP
 */
[CreateAssetMenu(menuName = "Classes/Passives/XenoPassive")]
public class XenoPassive : SOClassPassive
{
    public float maxHpPercentBonus = 0.01f;        // 1% vida máxima
    public float missingHpPercentBonus = 0.02f;    // 2% vida faltante

    public override void Activate(PlayerStats stats)
    {
        // Iniciamos el cálculo dinámico (sin guardar estado en el SO)
        stats.StartCoroutine(UpdateBonus(stats));
    }

    public override void Deactivate(PlayerStats stats)
    {
        // Al desactivar, restauramos los valores base
        stats.caCDmg.Current = stats.baseCaCDmgRuntime;
        stats.distDmg.Current = stats.baseDistDmgRuntime;
    }

    /*
     * Recalcula cada frame el bonus de daño de Xeno
     * sin pisar el daño base ni otros modificadores
     */
    private IEnumerator UpdateBonus(PlayerStats stats)
    {
        while (true)
        {
            float bonusFromMaxHp =
                stats.maxHP.Current * maxHpPercentBonus;

            float bonusFromMissingHp =
                (stats.maxHP.Current - stats.currentHp) * missingHpPercentBonus;

            float totalBonus = bonusFromMaxHp + bonusFromMissingHp;

            stats.caCDmg.Current = stats.baseCaCDmgRuntime + totalBonus;
            stats.distDmg.Current = stats.baseDistDmgRuntime + totalBonus;

            yield return null; // se recalcula constantemente
        }
    }
}