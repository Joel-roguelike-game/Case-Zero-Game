using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Classes/Passives/CaelaPassive")]
public class CaelaPassive : SOClassPassive
{
    [Header("Parry Base Modifiers")]
    [Range(0f, 1f)] public float baseParryCooldownReduction = 0.20f;
    [Range(0f, 1f)] public float baseParryCostReduction = 0.50f;

    [Header("Willpower Charges")]
    public int maxCharges = 3;
    public float chargeDuration = 6f;

    [Header("Per Charge Bonuses")]
    public float cacBonusPerCharge = 0.30f;
    public float distBonusPerCharge = 0.25f;
    public float lifestealPerCharge = 0.025f;
    public float parryCooldownPerCharge = 0.10f;

    private class Charge
    {
        public float expiry;
    }

    private class State
    {
        public List<Charge> charges = new();
        public int appliedStacks;
        public Coroutine tickRoutine;
        public float baseParryCooldown;
        public float baseParryStaminaCost;
    }

    private readonly Dictionary<PlayerStats, State> states = new();

    // =========================
    // ACTIVATE / DEACTIVATE
    // =========================

    public override void Activate(PlayerStats stats)
    {
        Debug.Log("[CaelaPassive] ✅ Espíritu Luchador activado");
        PlayerMovement movement =  stats.GetComponent<PlayerMovement>();
        CombatEvents.OnParrySuccess += OnParrySuccess;
        states[stats] = new State();

        movement = stats.GetComponent<PlayerMovement>();
        movement.parryCooldown -= movement.parryCooldown * baseParryCooldownReduction;
        movement.parryStaminaCost -= movement.parryStaminaCost * baseParryCostReduction;
        Debug.Log(
            $"[CaelaPassive] Base parry CD reducido un {baseParryCooldownReduction * 100}%"
        );
        Debug.Log(
            movement.parryCooldown +" | "+movement.parryStaminaCost
        );
    }

    public override void Deactivate(PlayerStats stats)
    {
        Debug.Log("[CaelaPassive] ❌ Desactivando Espíritu Luchador");

        CombatEvents.OnParrySuccess -= OnParrySuccess;

        if (!states.TryGetValue(stats, out State state))
            return;

        RemoveBonuses(stats, state.appliedStacks);

        PlayerMovement movement = stats.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.parryCooldown = state.baseParryCooldown;
            movement.parryStaminaCost = state.baseParryStaminaCost;
            movement.parryCooldown = 2.85f;
        }

        if (state.tickRoutine != null)
            stats.StopCoroutine(state.tickRoutine);

        states.Remove(stats);
    }

    // =========================
    // PARRY SUCCESS
    // =========================

    private void OnParrySuccess(PlayerStats stats)
    {
        if (!states.TryGetValue(stats, out State state))
            return;

        CleanupExpired(state);

        if (state.charges.Count >= maxCharges)
        {
            state.charges.Sort((a, b) => a.expiry.CompareTo(b.expiry));
            state.charges[0].expiry = Time.time + chargeDuration;

            Debug.Log("[CaelaPassive] 🔁 Carga renovada (límite alcanzado)");
        }
        else
        {
            state.charges.Add(new Charge { expiry = Time.time + chargeDuration });
            Debug.Log("[CaelaPassive] ➕ Nueva carga obtenida");
        }

        if (state.tickRoutine == null)
            state.tickRoutine = stats.StartCoroutine(ChargeTick(stats, state));

        UpdateBonuses(stats, state);
    }

    // =========================
    // CHARGE HANDLING
    // =========================

    private IEnumerator ChargeTick(PlayerStats stats, State state)
    {
        Debug.Log("[CaelaPassive] ⏱ Iniciando control de expiración de cargas");

        while (state.charges.Count > 0)
        {
            CleanupExpired(state);
            UpdateBonuses(stats, state);
            yield return new WaitForSeconds(0.2f);
        }

        state.tickRoutine = null;
        Debug.Log("[CaelaPassive] ⌛ Todas las cargas han expirado");
    }

    private void CleanupExpired(State state)
    {
        for (int i = state.charges.Count - 1; i >= 0; i--)
        {
            if (Time.time >= state.charges[i].expiry)
            {
                state.charges.RemoveAt(i);
                Debug.Log("[CaelaPassive] ⌛ Carga expirada");
            }
        }
    }

    // =========================
    // BONUS APPLICATION
    // =========================

    private void UpdateBonuses(PlayerStats stats, State state)
    {
        int newStacks = state.charges.Count;
        int delta = newStacks - state.appliedStacks;

        if (delta == 0)
            return;

        ApplyBonuses(stats, delta);
        state.appliedStacks = newStacks;

        PlayerMovement movement = stats.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.CurrentParryCooldownMultiplier =
                Mathf.Clamp(
                    1f
                    - baseParryCooldownReduction
                    - (newStacks * parryCooldownPerCharge),
                    0.4f,
                    1f
                );

            Debug.Log(
                $"[CaelaPassive] 🛡 Parry CD Multiplier actualizado: " +
                $"{movement.CurrentParryCooldownMultiplier}"
            );
        }

        Debug.Log(
            $"[CaelaPassive] 🔼 Cargas activas: {newStacks}/{maxCharges}"
        );
    }

    private void ApplyBonuses(PlayerStats stats, int stacks)
    {
        stats.caCDmg.AddPercent(cacBonusPerCharge * stacks);
        stats.distDmg.AddPercent(distBonusPerCharge * stacks);
        stats.lifestealPercent.AddFlat(lifestealPerCharge * stacks);

        stats.caCDmg.Recalculate();
        stats.distDmg.Recalculate();
        stats.lifestealPercent.Recalculate();

        Debug.Log(
            $"[CaelaPassive] Bonus aplicados ({stacks}): " +
            $"CaC {(cacBonusPerCharge * stacks) * 100}% | " +
            $"Dist {(distBonusPerCharge * stacks) * 100}% | " +
            $"LS {(lifestealPerCharge * stacks) * 100}%"
        );
    }

    private void RemoveBonuses(PlayerStats stats, int stacks)
    {
        if (stacks == 0) return;

        Debug.Log($"[CaelaPassive] ❌ Eliminando bonus ({stacks} stacks)");
        ApplyBonuses(stats, -stacks);
    }

    // =========================
    // DEBUG / UI
    // =========================

    public int GetCharges(PlayerStats stats)
    {
        return states.TryGetValue(stats, out State s) ? s.charges.Count : 0;
    }
}