using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Classes/Passives/LeveePassive")]
public class LeveePassive : SOClassPassive
{
    [Header("Base Crit Bonuses")]
    public float critChanceBonus = 10f;
    public float critDamageBonus = 50f;

    [Header("Deductor Settings")]
    public float windowDuration = 2f;
    public float healPercent = 0.02f;
    public float baseMoveSpeedPercent = 0.15f;
    public float extraMoveSpeedPerStack = 0.05f;
    public int maxStacks = 5;
    public float storedDamagePercent = 0.20f;
    public float cooldown = 5f;

    public LeveeActive leveeActive;

    private class State
    {
        public float windowEnd;
        public float storedCritDamage;
        public int stacks;
        public bool healUsed;
        public float moveSpeedFlat;
        public Coroutine routine;
    }

    private readonly Dictionary<PlayerStats, State> states = new();

    public override void Activate(PlayerStats stats)
    {
        Debug.Log("[LeveePassive] ACTIVADA");

        stats.critChance.AddFlat(critChanceBonus);
        stats.critDamage.AddFlat(critDamageBonus);
        CombatEvents.OnPlayerHit += OnHit;
    }

    public override void Deactivate(PlayerStats stats)
    {
        Debug.Log("[LeveePassive] DESACTIVADA");

        stats.critChance.AddFlat(-critChanceBonus);
        stats.critDamage.AddFlat(-critDamageBonus);
        CombatEvents.OnPlayerHit -= OnHit;
        states.Remove(stats);
    }

    private void OnHit(PlayerStats stats, DamageContext ctx)
    {
        if (!ctx.isCrit)
            return;

        if (!states.TryGetValue(stats, out State s))
            states[stats] = s = new State();

        // 🔹 Cooldown interno en PlayerStats
        if (!stats.IsPassiveReady(this))
            return;

        bool activeRefresh = leveeActive && leveeActive.IsRunning(stats);

        // ───────────────
        // Ventana de 2s
        // ───────────────
        if (Time.time > s.windowEnd)
        {
            Reset(stats, s);
            s.windowEnd = Time.time + windowDuration;
        }
        else if (activeRefresh)
        {
            // Refresca ventana si la activa está corriendo
            s.windowEnd = Time.time + windowDuration;
        }

        // ───────────────
        // Acumulación de daño y MS
        // ───────────────
        s.storedCritDamage += ctx.damage;

        if (!s.healUsed)
        {
            float heal = stats.maxHP.Current * healPercent;
            stats.currentHp = Mathf.Min(stats.currentHp + heal, stats.maxHP.Current);
            s.healUsed = true;
        }

        s.stacks = Mathf.Min(s.stacks + 1, maxStacks);

        float percent = baseMoveSpeedPercent + (s.stacks - 1) * extraMoveSpeedPerStack;
        float flat = stats.moveSpeed.Current * percent;

        stats.moveSpeed.AddFlat(-s.moveSpeedFlat);
        stats.moveSpeed.AddFlat(flat);
        s.moveSpeedFlat = flat;

        // Reinicia coroutine de fin de ventana
        if (s.routine != null)
            stats.StopCoroutine(s.routine);

        s.routine = stats.StartCoroutine(WindowEnd(stats, s));
    }

    private IEnumerator WindowEnd(PlayerStats stats, State s)
    {
        yield return new WaitUntil(() => Time.time >= s.windowEnd);

        // Quitar velocidad aplicada
        stats.moveSpeed.AddFlat(-s.moveSpeedFlat);

        // Daño adicional equivalente al 20% del daño crítico acumulado
        float aoe = s.storedCritDamage * storedDamagePercent;
        Debug.Log($"[LeveePassive] AOE → {aoe}");

        EnemyCombat[] enemies = Object.FindObjectsByType<EnemyCombat>(FindObjectsSortMode.None);
        foreach (EnemyCombat e in enemies)
        {
            if (!e || e.health.isDead)
                continue;

            DamageContext ctx = new DamageContext
            {
                damage = aoe,
                isCrit = false,
                source = stats
            };

            e.ReceiveHit(ctx, 0f, 1f);
        }

        // 🔹 Activar cooldown interno de 5s
        stats.SetPassiveCooldown(this, cooldown);

        // Limpiar datos de la ventana
        s.moveSpeedFlat = 0f;
        s.storedCritDamage = 0f;
        s.stacks = 0;
        s.healUsed = false;
        s.routine = null;
    }

    private void Reset(PlayerStats stats, State s)
    {
        s.moveSpeedFlat = 0f;
        s.storedCritDamage = 0f;
        s.stacks = 0;
        s.healUsed = false;
    }
}
