using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

/*
 * LeveePassive
 *
 * Pasiva: DEDUCTOR
 * Al realizar un golpe crítico:
 * - Aumenta velocidad de movimiento en 20% durante 1s
 * - Cura 5% de vida máxima
 * - Tiene 4s de cooldown
 */
[CreateAssetMenu(fileName = "LeveePassive", menuName = "Classes/Passives/LeveePassive")]
public class LeveePassive : SOClassPassive
{
    private float lastProcTime = -999f; // Tiempo de último proc
    private const float cooldown = 4f;

    public override void Activar(PlayerStats stats)
    {
        // Evita que se active antes del cooldown
        if (Time.time < lastProcTime + cooldown) return;
        lastProcTime = Time.time;

        // Lanza la rutina que aplica efectos temporales
        stats.StartCoroutine(ApplyPassiveEffects(stats));
    }

    /*
     * Aplica la velocidad extra y curación de manera temporal
     */
    private IEnumerator ApplyPassiveEffects(PlayerStats stats)
    {
        // Guardamos la velocidad original
        float originalSpeed = stats.moveSpeed.Current;

        // Aumentamos la velocidad en 20%
        stats.moveSpeed.Current *= 1.2f;

        // Curamos 5% de la vida máxima sin superar el máximo
        stats.currentHp = Mathf.Min(stats.currentHp + stats.maxHP.Current * 0.05f, stats.maxHP.Current);

        // Esperamos 1 segundo de duración de la pasiva
        yield return new WaitForSeconds(1f);

        // Restauramos velocidad original
        stats.moveSpeed.Current = originalSpeed;
    }
    
    
}