using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Classes/Passives/LeveePassive")]
public class LeveePassive : SOClassPassive
{
    public float moveSpeedBonus = 0.2f;
    public float duration = 1f;
    public float healPercent = 0.05f;
    public float cooldown = 5f;

    private readonly Dictionary<PlayerStats, float> lastProcTimes = new(); //Map<> 


    public override void Activate(PlayerStats stats)
    {
        CombatEvents.OnPlayerHit += OnHit;
    }

    public override void Deactivate(PlayerStats stats)
    {
        CombatEvents.OnPlayerHit -= OnHit;
        lastProcTimes.Remove(stats);
    }

    private void OnHit(PlayerStats stats, DamageResult result)
    {
        if (!result.isCrit) return;

        if (!lastProcTimes.TryGetValue(stats, out float lastTime))
            lastTime = -999f;

        if (Time.time < lastTime + cooldown)
        {
            Debug.Log("esta en cooldown");
            return;
        }
            

        lastProcTimes[stats] = Time.time;
        stats.StartCoroutine(Apply(stats));
    }

    private IEnumerator Apply(PlayerStats stats)
    {
        //Debug.Log("Aplicando");

        float heal = stats.maxHP.Current * healPercent;
        stats.currentHp = Mathf.Min(stats.currentHp + heal, stats.maxHP.Current);

        stats.moveSpeed.AddPercent(moveSpeedBonus);

        yield return new WaitForSeconds(duration);
        //Debug.Log("Eliminando buff");

        stats.moveSpeed.PercentBonus -= moveSpeedBonus;
        stats.moveSpeed.Recalculate();
    }

}