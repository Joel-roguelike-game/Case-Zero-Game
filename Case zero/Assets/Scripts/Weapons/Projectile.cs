// Projectile.cs
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 100f;
    public Vector2 direction;
    public float lifeTime = 3f;

    public PlayerStats owner;
    public bool isEchoProjectile = false;

    private float timer;
    private bool hasHit;

    private void Start()
    {
        // Solo el jugador dispara evento
        if (!isEchoProjectile && owner != null)
        {
            AttackContext ctx = new AttackContext(direction, AttackType.Ranged, owner, AttackSource.Player);
            CombatEvents.OnPlayerAttack?.Invoke(owner, ctx);
        }
    }

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
        if (!enemy || enemy.health.isDead) return;

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

        if (isEchoProjectile)
            ctx.damage *= 1.25f;

        enemy.ReceiveHit(
            ctx,
            owner.weaponRanged.stabilityBreak,
            owner.stabilityMultiplier.Current
        );

        owner.GetComponent<PlayerHealth>()?.TryApplyLifesteal();
        Destroy(gameObject);
    }

}