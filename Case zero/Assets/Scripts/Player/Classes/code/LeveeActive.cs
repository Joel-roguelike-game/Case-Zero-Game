using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Classes/Actives/LeveeActive")]
public class LeveeActive : SOClassActive
{
    [Header("Active Settings")]
    public float duration = 8f;

    [Tooltip("20% base damage taken")]
    public float baseDamageBonus = 0.20f;

    [Tooltip("Extra % based on current move speed (relative)")]
    public float moveSpeedScaling = 0.20f;

    [Header("Kill Reward")]
    public float focusOnKill = 25f;
    public float critChanceOnKill = 20f;
    public float critBuffDuration = 10f;

    private readonly HashSet<PlayerStats> activePlayers = new();
    private EnemyCombat target;

    // 🔹 Control del buff de crítico para que no se acumule
    private readonly Dictionary<PlayerStats, Coroutine> critBuffRoutines = new();

    public bool IsRunning(PlayerStats stats)
        => activePlayers.Contains(stats);

    public override void Activar(PlayerStats stats)
    {
        Debug.Log($"[LeveeActive] Try activate | Focus:{stats.currentFocus}");

        if (!stats.IsActiveReady(this))
        {
            Debug.Log("[LeveeActive] ❌ En cooldown interno");
            return;
        }


        target = FindEnemyUnderCursor();
        if (!target)
        {
            Debug.Log("[LeveeActive] ❌ Sin objetivo");
            return;
        }

        if (!stats.ConsumeFocus(focusCost))
        {
            Debug.Log("[LeveeActive] ❌ Focus insuficiente");
            return;
        }
        
        stats.SetActiveCooldown(this, duration); // similar a BladeActive
        stats.StartCoroutine(Run(stats));
    }

    private IEnumerator Run(PlayerStats stats)
    {
        activePlayers.Add(stats);
        Debug.Log("[LeveeActive] ACTIVA RUNNING");

        float currentBonus = CalculateBonus(stats);
        target.AddDamageTakenModifier(this, currentBonus);

        Debug.Log($"[LeveeActive] Vulnerability +{currentBonus * 100f:F1}%");

        float end = Time.time + duration;
        bool killed = false;

        while (Time.time < end)
        {
            if (!target || target.health.isDead)
            {
                killed = true;
                break;
            }

            // 🔁 Recalcular vulnerabilidad si cambia la MS
            float newBonus = CalculateBonus(stats);
            target.AddDamageTakenModifier(this, newBonus);

            yield return null;
        }

        if (target)
            target.RemoveDamageTakenModifier(this);

        activePlayers.Remove(stats);
        Debug.Log("[LeveeActive] ACTIVA END");

        if (killed)
        {
            stats.currentFocus = Mathf.Min(
                stats.currentFocus + focusOnKill,
                stats.maxFocus.Current
            );

            Debug.Log($"[LeveeActive] Kill bonus → +{focusOnKill} Focus");

            // ✅ Aplicar / refrescar buff de crítico
            ApplyCritBuff(stats);
        }
    }

    private float CalculateBonus(PlayerStats stats)
    {
        float baseMS = stats.moveSpeed.Base;
        float currentMS = stats.moveSpeed.Current;

        float relativeSpeed =
            baseMS > 0f ? (currentMS / baseMS) - 1f : 0f;

        float bonus =
            baseDamageBonus +
            (relativeSpeed * moveSpeedScaling);

        return Mathf.Max(0f, bonus);
    }

    // 🔹 Buff de crítico NO acumulable, refrescable
    private void ApplyCritBuff(PlayerStats stats)
    {
        // Si ya había un buff activo → eliminarlo
        if (critBuffRoutines.TryGetValue(stats, out Coroutine routine))
        {
            stats.StopCoroutine(routine);
            stats.critChance.AddFlat(-critChanceOnKill);
            critBuffRoutines.Remove(stats);

            Debug.Log("[LeveeActive] Crit buff refrescado");
        }

        Coroutine newRoutine = stats.StartCoroutine(CritBuff(stats));
        critBuffRoutines[stats] = newRoutine;
    }

    private IEnumerator CritBuff(PlayerStats stats)
    {
        Debug.Log($"[LeveeActive] +{critChanceOnKill}% crit for {critBuffDuration}s");

        stats.critChance.AddFlat(critChanceOnKill);

        yield return new WaitForSeconds(critBuffDuration);

        stats.critChance.AddFlat(-critChanceOnKill);
        critBuffRoutines.Remove(stats);

        Debug.Log("[LeveeActive] Crit buff ended");
    }

    private EnemyCombat FindEnemyUnderCursor()
    {
        Vector3 m = Mouse.current.position.ReadValue();
        Vector3 w = Camera.main.ScreenToWorldPoint(
            new Vector3(m.x, m.y, 10f)
        );

        Collider2D hit = Physics2D.OverlapCircle(w, 0.1f);
        return hit ? hit.GetComponent<EnemyCombat>() : null;
    }
}
