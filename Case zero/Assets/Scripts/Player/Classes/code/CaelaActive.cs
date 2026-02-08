using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Classes/Actives/CaelaActive")]
public class CaelaActive : SOClassActive
{
    public float duration = 3f;

    // Slow al 50%
    public float moveSpeedSlowPercent = -0.5f;

    public float cacBonusPerCharge = 0.5f;
    public float distBonusPerCharge = 0.4f;

    public override void Activar(PlayerStats stats)
    {
        Debug.Log("[CaelaActive] Intentando activar La Vigilante de Ephidra");

        if (!stats.ConsumeFocus(focusCost))
        {
            Debug.Log("[CaelaActive] ❌ No hay focus suficiente");
            return;
        }

        Debug.Log("[CaelaActive] ✅ Activada. Focus restante: " + stats.currentFocus);
        stats.StartCoroutine(Run(stats));
    }

    private IEnumerator Run(PlayerStats stats)
    {
        PlayerHealth health = stats.GetComponent<PlayerHealth>();
        CaelaPassive passive = stats.classPassive as CaelaPassive;

        if (passive == null)
        {
            Debug.LogError("[CaelaActive] ❌ Pasiva de Caela no encontrada");
            yield break;
        }

        Debug.Log("[CaelaActive] ▶ Entrando en estado de bloqueo (" + duration + "s)");

        // === SLOW ===
        stats.moveSpeed.AddPercent(moveSpeedSlowPercent);
        stats.moveSpeed.Recalculate();
        Debug.Log("[CaelaActive] Slow aplicado. MoveSpeed actual: " + stats.moveSpeed.Current);

        // === PARRY REAL ===
        health.StartParryInvulnerability(duration);
        Debug.Log("[CaelaActive] Invulnerabilidad/parry real activo");

        yield return new WaitForSeconds(duration);

        // === QUITAMOS SLOW ===
        stats.moveSpeed.AddPercent(-moveSpeedSlowPercent);
        stats.moveSpeed.Recalculate();
        Debug.Log("[CaelaActive] Slow eliminado. MoveSpeed restaurado: " + stats.moveSpeed.Current);

        int charges = passive.GetCharges(stats);
        Debug.Log("[CaelaActive] Cargas al finalizar bloqueo: " + charges);

        if (charges <= 0)
        {
            Debug.Log("[CaelaActive] No hay cargas → no se aplica bonus ofensivo");
            yield break;
        }

        float cacPercent = cacBonusPerCharge * charges;
        float distPercent = distBonusPerCharge * charges;

        Debug.Log(
            $"[CaelaActive] Preparando bonus NEXT HIT → CaC +{cacPercent * 100}% | Dist +{distPercent * 100}%"
        );

        stats.StartCoroutine(ApplyNextHitBonus(stats, cacPercent, distPercent));
    }

    private IEnumerator ApplyNextHitBonus(
        PlayerStats stats,
        float cacPercent,
        float distPercent
    )
    {
        // Aplicamos bonus
        stats.caCDmg.AddPercent(cacPercent);
        stats.distDmg.AddPercent(distPercent);
        stats.caCDmg.Recalculate();
        stats.distDmg.Recalculate();

        Debug.Log(
            $"[CaelaActive] BONUS APLICADO → CaC: {stats.caCDmg.Current} | Dist: {stats.distDmg.Current}"
        );

        bool consumed = false;

        void OnHit(PlayerStats source, DamageContext ctx)
        {
            if (source != stats || consumed)
                return;

            consumed = true;

            Debug.Log(
                $"[CaelaActive] 💥 BONUS CONSUMIDO por golpe. Daño final: {ctx.damage}"
            );

            stats.caCDmg.AddPercent(-cacPercent);
            stats.distDmg.AddPercent(-distPercent);
            stats.caCDmg.Recalculate();
            stats.distDmg.Recalculate();

            Debug.Log(
                $"[CaelaActive] Bonus retirado → CaC: {stats.caCDmg.Current} | Dist: {stats.distDmg.Current}"
            );

            CombatEvents.OnPlayerHit -= OnHit;
        }

        CombatEvents.OnPlayerHit += OnHit;

        yield return new WaitForSeconds(5f);

        if (!consumed)
        {
            Debug.Log("[CaelaActive] ⏱ BONUS EXPIRADO sin golpear");

            stats.caCDmg.AddPercent(-cacPercent);
            stats.distDmg.AddPercent(-distPercent);
            stats.caCDmg.Recalculate();
            stats.distDmg.Recalculate();

            CombatEvents.OnPlayerHit -= OnHit;
        }
    }
}
