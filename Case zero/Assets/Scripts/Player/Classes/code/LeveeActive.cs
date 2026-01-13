using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

/*
 * LeveeActive
 * 
 * Activa: HYPERCONCENTRACION
 * - Si hay un enemigo bajo el cursor, recibe +30% daño durante 10s
 * - Matar al objetivo antes de que termine devuelve la mitad del focus y aumenta prob. crítica en 20% por 10s
 */
[CreateAssetMenu(fileName = "LeveeActive", menuName = "Classes/Actives/LeveeActive")]
public class LeveeActive : SOClassActive
{
    public float duration = 10f;
    public float damageBonusPercent = 0.30f; // +30%
    public float critChanceBonus = 20f;      // +20% crit
    public float killBuffDuration = 10f;
    public float focusOnKill = 25f;

    private EnemyCombat targetEnemy;

    public override void Activar(PlayerStats stats)
    {
        targetEnemy = FindEnemyUnderCursor();
        if (targetEnemy == null)
            return;

        stats.StartCoroutine(ApplyActive(stats));
    }

    /*
     * Rutina que aplica el efecto sobre el enemigo seleccionado
     */
    private IEnumerator ApplyActive(PlayerStats stats)
    {
        float startTime = Time.time;

        // Aplicamos vulnerabilidad contra Levee
        targetEnemy.AddDamageTakenModifier(stats, damageBonusPercent);

        // Subimos la probabilidad de crítico usando AddFlat
        stats.critChance.AddFlat(critChanceBonus);

        bool targetKilled = false;

        while (Time.time < startTime + duration)
        {
            if (targetEnemy == null || targetEnemy.health == null)
                break;

            if (targetEnemy.health.isDead)
            {
                targetKilled = true;
                break;
            }

            yield return null;
        }

        // Quitamos vulnerabilidad
        if (targetEnemy != null)
            targetEnemy.RemoveDamageTakenModifier(stats);

        // Restauramos critChance
        stats.critChance.AddFlat(-critChanceBonus);

        // Si murió a tiempo → recompensa en focus
        if (targetKilled)
        {
            stats.currentFocus = Mathf.Min(
                stats.currentFocus + focusOnKill,
                stats.maxFocus.Current
            );
        }
    }

    /*
     * Detecta enemigo bajo cursor
     */
    private EnemyCombat FindEnemyUnderCursor()
    {
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(mouseScreenPos.x, mouseScreenPos.y, 10f)
        );

        Collider2D hit = Physics2D.OverlapCircle(mouseWorldPos, 0.1f);
        if (hit != null && hit.CompareTag("Enemy"))
            return hit.GetComponent<EnemyCombat>();

        return null;
    }
}
