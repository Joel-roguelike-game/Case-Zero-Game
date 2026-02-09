using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Classes/Passives/ZhexPassive")]
public class ZhexPassive : SOClassPassive
{
    private class State
    {
        public int rangedCount;
        public int meleeCount;
        public float nextRangedAllowed;
        public float nextMeleeAllowed;
    }

    private readonly Dictionary<PlayerStats, State> states = new();

    public override void Activate(PlayerStats stats)
    {
        states[stats] = new State();
        CombatEvents.OnPlayerAttack += OnAttack;

        Debug.Log("[ZHEX] Pasiva activada");
    }

    public override void Deactivate(PlayerStats stats)
    {
        CombatEvents.OnPlayerAttack -= OnAttack;
        states.Remove(stats);

        Debug.Log("[ZHEX] Pasiva desactivada");
    }

    private void OnAttack(PlayerStats stats, AttackContext ctx)
    {
        if (!states.TryGetValue(stats, out State s))
            return;

        // ===============================
        // ATAQUE A DISTANCIA – 6º HIT
        // ===============================
        if (ctx.type == AttackType.Ranged)
        {
            s.rangedCount++;
            Debug.Log($"[ZHEX] Ranged hit #{s.rangedCount}");

            if (s.rangedCount >= 6 && Time.time >= s.nextRangedAllowed)
            {
                s.rangedCount = 0;
                s.nextRangedAllowed = Time.time + 3f;

                // Flag del ataque especial (NO spawnea aquí)
                ctx.source = AttackSource.Echo;

                Debug.Log("[ZHEX] Disparo especial de sangre ACTIVADO");
            }
        }

        // ===============================
        // ATAQUE MELEE – 8º HIT
        // ===============================
        else if (ctx.type == AttackType.Melee)
        {
            s.meleeCount++;
            Debug.Log($"[ZHEX] Melee hit #{s.meleeCount}");

            if (s.meleeCount < 8 || Time.time < s.nextMeleeAllowed)
                return;

            s.meleeCount = 0;
            s.nextMeleeAllowed = Time.time + 3f;

            EnemyCombat enemy = ctx.owner
                ? ctx.owner.GetComponentInChildren<EnemyCombat>()
                : null;

            if (!enemy)
            {
                Debug.LogWarning("[ZHEX] No enemy detected for melee detonation");
                return;
            }

            EnemyDotController dots = enemy.GetComponent<EnemyDotController>();
            if (!dots)
            {
                Debug.LogWarning("[ZHEX] Enemy has no DotController");
                return;
            }

            int seconds = dots.CountTotalRemainingSeconds(5);
            float healPercent = Mathf.Min(seconds, 15) / 100f;

            float explosionDamage = dots.ExplodeAllDots();

            Debug.Log($"[ZHEX] Detonación DOT: {explosionDamage} dmg | Heal: {healPercent * 100f}%");

            DamageContext dmgCtx = DamageCalculator.CreatePureDamage(
                stats,
                explosionDamage,
                false
            );

            enemy.ReceiveHit(dmgCtx, 0f, 1f);

            PlayerHealth health = stats.GetComponent<PlayerHealth>();
            if (health)
            {
                health.HealPercent(healPercent);
                Debug.Log("[ZHEX] Curación aplicada");
            }
        }
    }
}
