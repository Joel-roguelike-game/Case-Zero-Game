using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "XenoActive", menuName = "Classes/Actives/XenoActive")]
public class XenoActive : SOClassActive
{
    public float duration = 10f;                 // Duración del escudo
    public float missingHpPercent = 0.3f;        // Escudo = 30% de la vida que falta
    public float healOnBreakPercent = 0.25f;     // Cura 25% de la vida actual si se rompe
    public float bonusMaxHpPercent = 0.25f;      // +25% vida máxima si el escudo aguanta

    private bool isActive = false;

    public override void Activar(PlayerStats stats)
    {
        if (Time.time < stats.lastActiveTime + cooldown) return;

        stats.lastActiveTime = Time.time;
        stats.StartCoroutine(ApplyActive(stats));
    }

    private IEnumerator ApplyActive(PlayerStats stats)
    {
        if (isActive) yield break;
        isActive = true;

        PlayerHealth health = stats.GetComponent<PlayerHealth>();

        // Calculamos la vida que falta
        float missingHp = stats.maxHP.Current - stats.currentHp;

        // Escudo como vida temporal
        float shield = missingHp * missingHpPercent;
        float initialHp = stats.currentHp;
        health.Heal(shield); // añadimos al total de vida actual como escudo

        bool shieldBroken = false;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Si la vida actual es menor que el valor inicial del escudo, significa que se rompió
            if (stats.currentHp < initialHp)
            {
                float lostShield = initialHp - stats.currentHp;
                shieldBroken = true;
                // Retiramos lo que quede del escudo
                stats.currentHp = Mathf.Max(stats.currentHp, 0f);
                // Curamos al jugador según porcentaje de vida actual
                health.Heal(stats.currentHp * healOnBreakPercent);
                break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Si el escudo no se rompió, otorgamos vida máxima temporal
        if (!shieldBroken)
        {
            float bonusMaxHp = stats.maxHP.Base * bonusMaxHpPercent;
            stats.maxHP.Current += bonusMaxHp;
            health.Heal(bonusMaxHp);

            yield return new WaitForSeconds(duration);

            stats.maxHP.Current -= bonusMaxHp;
            // Si la vida actual es mayor que la vida máxima, la ajustamos
            stats.currentHp = Mathf.Min(stats.currentHp, stats.maxHP.Current);
        }

        isActive = false;
    }
}
