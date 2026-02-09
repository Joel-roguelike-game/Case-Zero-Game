using UnityEngine;

public class ZhexBloodDot : BaseDotInstance
{
    private PlayerStats owner;
    private float damagePerTick;
    private float critChance;
    private float critDamage;

    public ZhexBloodDot(PlayerStats owner, float damagePerTick, float duration)
    {
        this.owner = owner;
        this.damagePerTick = damagePerTick;
        remainingDuration = duration;

        critChance = owner.critChance.Current;
        critDamage = owner.critDamage.Current;
    }

    public override void Tick(EnemyCombat enemy)
    {
        tickTimer += Time.deltaTime;
        if (tickTimer < 1f) return;

        tickTimer = 0f;
        remainingDuration -= 1f;

        bool isCrit = Random.value < critChance;
        float dmg = damagePerTick * (isCrit ? critDamage : 1f);

        DamageContext ctx = DamageCalculator.CreatePureDamage(
            owner,
            dmg,
            isCrit
        );

        enemy.ReceiveHit(ctx, 0f, 1f);

        DotDamageTextSpawner.Instance?.Spawn(
            enemy.transform.position + Vector3.down * 0.5f,
            Mathf.RoundToInt(dmg),
            isCrit
        );
    }

    public override float Explode()
    {
        float total = damagePerTick * Mathf.Ceil(remainingDuration);
        remainingDuration = 0f;
        return total;
    }
}