using UnityEngine;

public static class DamageCalculator
{
    public static DamageResult CalculatePlayerDamage(
        float flat,
        float percent,
        float playerDamage,
        float critChance,
        float critMult,
        bool isParry,
        float parryMult,
        bool stabilityBroken,
        float stabilityMult
    )
    {
        float baseDamage = flat + ((percent/100) * playerDamage);

        bool isCrit = Random.Range(0f, 100f) <= critChance;
        float critMultiplier = isCrit ? critMult / 100f : 1f;

        float extraMultiplier = Mathf.Max(
            isParry ? parryMult : 1f,
            stabilityBroken ? stabilityMult : 1f
        );

        float finalDamage = baseDamage * critMultiplier * extraMultiplier;

        return new DamageResult
        {
            damage = finalDamage,
            isCrit = isCrit
        };
    }
}