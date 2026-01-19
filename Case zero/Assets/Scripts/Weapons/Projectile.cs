using UnityEngine;

/*
 * Proyectil del jugador
 */
public class Projectile : MonoBehaviour
{
    public float speed = 100f;
    public Vector2 direction;
    public float lifeTime = 3f;

    public PlayerStats owner;

    private float timer;
    private bool hasHit;

    private void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= lifeTime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        EnemyCombat enemy = other.GetComponentInParent<EnemyCombat>();
        if (!enemy || enemy.health.isDead)
            return;

        hasHit = true;

        bool isParry = enemy.ConsumeParryAffected();

        DamageContext ctx = DamageCalculator.CalculatePlayerDamage(
            owner,
            owner.weaponRanged.flatDamage,
            owner.weaponRanged.damagePercent,
            owner.distDmg.Current,
            owner.critChance.Current,
            owner.critDamage.Current,
            isParry,
            owner.parryMultiplier.Current,
            enemy.GetComponent<EnemyStats>().stabilityBroken,
            owner.stabilityMultiplier.Current,
            null
        );

        Debug.Log(
            $"[Projectile] HIT → dmg:{ctx.damage} crit:{ctx.isCrit}"
        );

        enemy.ReceiveHit(
            ctx,
            owner.weaponRanged.stabilityBreak,
            owner.stabilityMultiplier.Current
        );

        owner.GetComponent<PlayerHealth>()
            ?.TryApplyLifesteal();

        Destroy(gameObject);
    }
}