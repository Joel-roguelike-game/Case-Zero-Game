using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Classes/Passives/LeveePassive")]
public class LeveePassive : SOClassPassive
{
    public float moveSpeedBonus = 0.2f;
    public float duration = 1f;
    public float healPercent = 0.05f;
    public float cooldown = 4f;

    private float lastProcTime = -999f;

    public override void Activate(PlayerStats stats)
    {
        CombatEvents.OnPlayerHit += OnHit;
    }

    public override void Deactivate(PlayerStats stats)
    {
        CombatEvents.OnPlayerHit -= OnHit;
    }

    private void OnHit(PlayerStats stats, DamageResult result)
    {
        if (!result.isCrit) return;
        if (Time.time < lastProcTime + cooldown) return;

        lastProcTime = Time.time;
        stats.StartCoroutine(Apply(stats));
    }

    private IEnumerator Apply(PlayerStats stats)
    {
        float heal = stats.maxHP.Current * healPercent;
        stats.currentHp = Mathf.Min(stats.currentHp + heal, stats.maxHP.Current);

        stats.moveSpeed.Current *= (1f + moveSpeedBonus);
        yield return new WaitForSeconds(duration);
        stats.moveSpeed.Current /= (1f + moveSpeedBonus);
    }
}