using UnityEngine;
/*Clase que se encarga de los calculos de daño del jugador. */
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
        float baseDamage = flat + ((percent/100) * playerDamage); //ddaño base

        bool isCrit = Random.Range(0f, 100f) <= critChance; //prob
        float critMultiplier = isCrit ? critMult / 100f : 1f; //si es critico,coje el multi, sino x1

        float extraMultiplier = Mathf.Max(
            isParry ? parryMult : 1f,
            stabilityBroken ? stabilityMult : 1f
        ); //coje el multiplicador mas alto: en el caso de que sea parry y rotura a la vez solo coje el mas alto, si no es ninguno, x1

        float finalDamage = baseDamage * critMultiplier * extraMultiplier; 

        return new DamageResult
        {
            damage = finalDamage,
            isCrit = isCrit //devuelvo esto para numero amarillo en critico maybe se podria cambiar tambien para parry/rotura
        };
    }
}