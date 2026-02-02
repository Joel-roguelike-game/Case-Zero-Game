using UnityEngine;
using System.Collections;

/*
 * BladeActive
 *
 * Activa: Cambio de alma
 * - Teleport al eco más cercano a caducar
 * - Explosión
 * - +30% MS durante 1s
 */
[CreateAssetMenu(menuName = "Classes/Actives/BladeActive")]
public class BladeActive : SOClassActive
{
    public float moveSpeedBonus = 0.30f;
    public float moveSpeedDuration = 1f;
    public float internalCooldown = 1f;

    private bool isRunning;

    public override void Activar(PlayerStats stats)
    {
        Debug.Log($"[BladeActive] Try activate | Focus:{stats.currentFocus}");

        if (isRunning)
        {
            Debug.Log("[BladeActive] ❌ En cooldown interno");
            return;
        }

        BladePassive passive =
            stats.classPassive as BladePassive;

        if (!passive)
            return;

        BladeEcho echo =
            passive.GetEchoClosestToExpire(stats);

        if (!echo)
        {
            Debug.Log("[BladeActive] ❌ Sin ecos");
            return;
        }

        if (!stats.ConsumeFocus(focusCost))
        {
            Debug.Log("[BladeActive] ❌ Focus insuficiente");
            return;
        }

        stats.StartCoroutine(Run(stats, passive, echo));
    }

    private IEnumerator Run(
        PlayerStats stats,
        BladePassive passive,
        BladeEcho echo
    )
    {
        isRunning = true;

        Debug.Log("[BladeActive] TELEPORT");

        stats.transform.position = echo.transform.position;

        passive.RemoveEcho(stats, echo);
        echo.Explode();

        float bonus = stats.moveSpeed.Base * moveSpeedBonus;
        stats.moveSpeed.AddFlat(bonus);

        yield return new WaitForSeconds(moveSpeedDuration);

        stats.moveSpeed.AddFlat(-bonus);

        yield return new WaitForSeconds(internalCooldown);

        isRunning = false;
        Debug.Log("[BladeActive] READY");
    }
}