using UnityEngine;
/*
 * DamageCalculator
 * Calcula el daño del jugador y genera un DamageContext
 */
public static class DamageCalculator
{
    
    public static DamageContext CalculatePlayerDamage(
        PlayerStats playerStats,    // <--- PASAMOS EL PLAYERSTATS
        float flat,
        float percent,
        float playerDamage,
        float critChance,
        float critMult,
        bool isParry,
        float parryMult,
        bool stabilityBroken,
        float stabilityMult,
        SOClassActive activeSource
    )
    {
        float baseDamage = flat + ((percent/100) * playerDamage); //daño base

        bool isCrit = Random.Range(0f, 100f) <= critChance; //prob

        float critMultiplier = isCrit ? critMult / 100f : 1f; //si es critico,coje el multi, sino x1

        float extraMultiplier = Mathf.Max(
            isParry ? parryMult : 1f,
            stabilityBroken ? stabilityMult : 1f
        ); //coje el multiplicador mas alto: en el caso de que sea parry y rotura a la vez solo coje el mas alto, si no es ninguno, x1

        float finalDamage = baseDamage * critMultiplier * extraMultiplier;

        DamageContext ctx = new DamageContext
        {
            damage = finalDamage,
            isCrit = isCrit,
            source = playerStats,
            activeSource = activeSource
        };
        
        CombatEvents.OnPlayerHit?.Invoke(playerStats, ctx);

        return ctx;
    }
    public static DamageContext CreatePureDamage(
        PlayerStats source,
        float damage,
        bool isCrit,
        SOClassActive activeSource = null
    )
    {
        DamageContext ctx = new DamageContext
        {
            damage = damage,
            isCrit = isCrit,
            source = source,
            activeSource = activeSource
        };

        CombatEvents.OnPlayerHit?.Invoke(source, ctx);
        return ctx;
    }

}