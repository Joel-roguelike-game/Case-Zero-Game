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
        public float cooldownEnd;
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

        Debug.Log($"[LeveePassive] CRIT HIT → {ctx.damage}");

        if (!states.TryGetValue(stats, out State s))
            states[stats] = s = new State();

        if (Time.time < s.cooldownEnd)
        {
            Debug.Log("[LeveePassive] En cooldown");
            return;
        }

        bool activeRefresh = leveeActive && leveeActive.IsRunning(stats);

        if (Time.time > s.windowEnd && !activeRefresh)
        {
            Debug.Log("[LeveePassive] Nueva ventana");
            Reset(stats, s); // Solo limpia datos, NO toca moveSpeed
        }

        s.windowEnd = Time.time + windowDuration;
        s.storedCritDamage += ctx.damage;

        if (!s.healUsed)
        {
            float heal = stats.maxHP.Current * healPercent;
            stats.currentHp = Mathf.Min(stats.currentHp + heal, stats.maxHP.Current);
            s.healUsed = true;

            Debug.Log($"[LeveePassive] Heal {heal}");
        }

        s.stacks = Mathf.Min(s.stacks + 1, maxStacks);

        float percent = baseMoveSpeedPercent + (s.stacks - 1) * extraMoveSpeedPerStack;
        float flat = stats.moveSpeed.Current * percent;

        // Aplica MS: quita el flat anterior y aplica el nuevo
        stats.moveSpeed.AddFlat(-s.moveSpeedFlat);
        stats.moveSpeed.AddFlat(flat);
        s.moveSpeedFlat = flat;

        Debug.Log($"[LeveePassive] MS stacks:{s.stacks} flat:{flat}");

        if (s.routine != null)
            stats.StopCoroutine(s.routine);

        s.routine = stats.StartCoroutine(WindowEnd(stats, s));
    }

    private IEnumerator WindowEnd(PlayerStats stats, State s)
    {
        yield return new WaitUntil(() => Time.time >= s.windowEnd);

        // SOLO quitar la velocidad aplicada, no tocar Reset
        stats.moveSpeed.AddFlat(-s.moveSpeedFlat);

        float aoe = s.storedCritDamage * storedDamagePercent;
        Debug.Log($"[LeveePassive] AOE → {aoe}");

        // ✅ Usando la nueva API para evitar el warning
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

        s.cooldownEnd = Time.time + cooldown;

        // Limpiar datos sin afectar MS
        s.moveSpeedFlat = 0f;
        s.storedCritDamage = 0f;
        s.stacks = 0;
        s.healUsed = false;

        Debug.Log($"[LeveePassive] Ventana finalizada, cooldown hasta {s.cooldownEnd}");
    }

    // Reset solo limpia los datos, NO toca la velocidad
    private void Reset(PlayerStats stats, State s)
    {
        s.moveSpeedFlat = 0f;
        s.storedCritDamage = 0f;
        s.stacks = 0;
        s.healUsed = false;
    }
}
