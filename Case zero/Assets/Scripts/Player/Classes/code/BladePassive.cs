using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Pasiva de Blade
/// Genera ecos al hacer dash y hace que imiten ataques del jugador
/// </summary>
[CreateAssetMenu(menuName = "Classes/Passives/BladePassive")]
public class BladePassive : SOClassPassive
{
    public BladeEcho echoPrefab;
    public int maxEchos = 2;

    private readonly Dictionary<PlayerStats, List<BladeEcho>> echoes = new();

    public override void Activate(PlayerStats stats)
    {
        echoes[stats] = new List<BladeEcho>();
        CombatEvents.OnDash += OnDash;
        CombatEvents.OnPlayerAttack += OnPlayerAttack;
    }

    public override void Deactivate(PlayerStats stats)
    {
        CombatEvents.OnDash -= OnDash;
        CombatEvents.OnPlayerAttack -= OnPlayerAttack;

        if (echoes.TryGetValue(stats, out var list))
        {
            foreach (var e in list)
                if (e) Object.Destroy(e.gameObject);
        }

        echoes.Remove(stats);
    }

    /// <summary>
    /// Genera un eco al hacer dash
    /// </summary>
    private void OnDash(PlayerStats stats)
    {
        if (!echoes.TryGetValue(stats, out var list)) return;

        while (list.Count >= maxEchos)
        {
            BladeEcho old = list[0];
            list.RemoveAt(0);
            if (old) Object.Destroy(old.gameObject);
        }

        BladeEcho echo = Object.Instantiate(
            echoPrefab,
            stats.transform.position,
            Quaternion.identity
        );

        echo.Init(stats);
        list.Add(echo);
    }

    /// <summary>
    /// Cuando el jugador ataca, los ecos imitan el ataque
    /// </summary>
    private void OnPlayerAttack(PlayerStats stats, AttackContext ctx)
    {
        if (ctx.source != AttackSource.Player) return;
        if (!echoes.TryGetValue(stats, out var list)) return;

        list.RemoveAll(e => e == null);

        foreach (var e in list)
            e.MimicAttack(ctx);
    }

    public BladeEcho GetEchoClosestToExpire(PlayerStats stats)
    {
        if (!echoes.TryGetValue(stats, out var list) || list.Count == 0)
            return null;

        BladeEcho best = null;
        float min = float.MaxValue;

        foreach (var e in list)
        {
            if (!e) continue;
            if (e.RemainingTime < min)
            {
                min = e.RemainingTime;
                best = e;
            }
        }

        return best;
    }

    public void RemoveEcho(PlayerStats stats, BladeEcho echo)
    {
        if (echoes.TryGetValue(stats, out var list))
            list.Remove(echo);
    }
}
