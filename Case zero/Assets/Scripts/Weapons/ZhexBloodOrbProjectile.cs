using UnityEngine;

public class ZhexBloodOrbProjectile : Projectile
{
    private new void OnTriggerEnter2D(Collider2D other)
    {
        // Ejecutamos el comportamiento normal del proyectil
        base.GetType()
            .GetMethod("OnTriggerEnter2D",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
            ?.Invoke(this, new object[] { other });

        EnemyCombat enemy = other.GetComponentInParent<EnemyCombat>();
        if (!enemy || owner == null) return;

        EnemyDotController dots = enemy.GetComponent<EnemyDotController>();
        if (!dots) return;

        float dotDamage =
            (owner.weaponRanged.damagePercent / 3f) *
            owner.distDmg.Current;

        dots.AddDot(new ZhexBloodDot(owner, dotDamage, 8f));
    }
}