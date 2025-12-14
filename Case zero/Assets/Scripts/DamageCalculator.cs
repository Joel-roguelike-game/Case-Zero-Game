using UnityEngine;

public static class DamageCalculator
{
    public static float CalculatePlayerDamage(
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
        float baseDamage = flat + (percent * playerDamage);

        bool crit = Random.Range(0f, 100f) <= critChance;
        float critMultiplier = crit ? critMult / 100f : 1f;

        float extraMultiplier = Mathf.Max(
            isParry ? parryMult : 1f,
            stabilityBroken ? stabilityMult : 1f
        );

        return baseDamage * critMultiplier * extraMultiplier;
    }
}