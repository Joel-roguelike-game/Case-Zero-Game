using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Classes/Actives/LeveeActive")]
public class LeveeActive : SOClassActive
{
    public float duration = 8f;
    public float baseDamageBonus = 0.20f;
    public float moveSpeedScaling = 0.20f;
    public float focusOnKill = 25f;

    private readonly HashSet<PlayerStats> activePlayers = new();
    private EnemyCombat target;

    public bool IsRunning(PlayerStats stats)
        => activePlayers.Contains(stats);

    public override void Activar(PlayerStats stats)
    {
        Debug.Log($"[LeveeActive] Try activate | Focus:{stats.currentFocus}");

        if (!stats.ConsumeFocus(focusCost))
        {
            Debug.Log("[LeveeActive] ❌ Focus insuficiente");
            return;
        }

        target = FindEnemyUnderCursor();
        if (!target)
        {
            Debug.Log("[LeveeActive] ❌ Sin objetivo");
            return;
        }

        stats.StartCoroutine(Run(stats));
    }

    private IEnumerator Run(PlayerStats stats)
    {
        activePlayers.Add(stats);
        Debug.Log("[LeveeActive] ACTIVA RUNNING");

        float bonus =
            baseDamageBonus +
            (stats.moveSpeed.Current * moveSpeedScaling);

        Debug.Log($"[LeveeActive] Vulnerability +{bonus * 100:F1}%");

        target.AddDamageTakenModifier(this, bonus);

        float end = Time.time + duration;
        bool killed = false;

        while (Time.time < end)
        {
            if (!target || target.health.isDead)
            {
                killed = true;
                break;
            }

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
        }
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
