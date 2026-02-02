using UnityEngine;

/// <summary>
/// Eco de Blade
/// Replica ataques del jugador pero:
/// - NO dispara eventos
/// - +25% daño total
/// - NO genera loops
/// </summary>
public class BladeEcho : MonoBehaviour
{
    public float lifetime = 5f;
    public float echoDamageMultiplier = 1.25f;

    [Header("Prefabs")]
    public GameObject meleeEchoPrefab;   // ← PREFAB con BoxCollider + MeleeSlashEco

    private PlayerStats owner;
    private float spawnTime;

    public float RemainingTime =>
        Mathf.Max(0f, lifetime - (Time.time - spawnTime));

    /// <summary>
    /// Inicializa el eco
    /// </summary>
    public void Init(PlayerStats stats)
    {
        owner = stats;
        spawnTime = Time.time;

        // Si luego quieres capa:
        // gameObject.layer = LayerMask.NameToLayer("Echo");
    }

    private void Update()
    {
        if (Time.time >= spawnTime + lifetime)
            Destroy(gameObject);
    }

    /// <summary>
    /// Replica un ataque del jugador
    /// </summary>
    public void MimicAttack(AttackContext originalCtx)
    {
        if (!owner) return;

        if (originalCtx.type == AttackType.Melee)
            SpawnEchoMelee(originalCtx.direction);
        else
            SpawnEchoRanged(originalCtx.direction);
    }

    /// <summary>
    /// Spawnea un melee del eco
    /// </summary>
    private void SpawnEchoMelee(Vector2 direction)
    {
        if (!meleeEchoPrefab) return;

        GameObject obj = Instantiate(
            meleeEchoPrefab,
            transform.position,
            Quaternion.identity
        );

        obj.tag = "EchoMelee";

        MeleeSlashEco eco = obj.GetComponent<MeleeSlashEco>();
        if (!eco) return;

        // 🔥 CLAVE: pasamos el TRANSFORM DEL ECO
        eco.Init(
            owner,
            transform,
            direction,
            echoDamageMultiplier
        );
    }

    /// <summary>
    /// Spawnea un proyectil eco reutilizando el prefab
    /// </summary>
    private void SpawnEchoRanged(Vector2 direction)
    {
        GameObject proj = Instantiate(
            owner.weaponRanged.weaponPrefab,
            transform.position,
            Quaternion.identity
        );

        Projectile p = proj.GetComponent<Projectile>();
        if (!p) return;

        p.owner = owner;
        p.direction = direction;
        p.isEchoProjectile = true;
    }

    public void Explode()
    {
        float radius = 3f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var col in hits)
        {
            EnemyCombat enemy = col.GetComponent<EnemyCombat>();
            if (!enemy || enemy.health.isDead) continue;

            bool isParry = enemy.ConsumeParryAffected();

            float cacDmg = owner.caCDmg.Current * 2f;
            float distDmg = owner.distDmg.Current * 2f;

            DamageContext ctx = DamageCalculator.CalculatePlayerDamage(
                owner,
                owner.weaponMelee.flatDamage + owner.weaponRanged.flatDamage,
                owner.weaponMelee.damagePercent + owner.weaponRanged.damagePercent,
                cacDmg + distDmg,
                owner.critChance.Current,
                owner.critDamage.Current,
                isParry,
                owner.parryMultiplier.Current,
                enemy.GetComponent<EnemyStats>().stabilityBroken,
                owner.stabilityMultiplier.Current,
                null
            );

            // +25% daño eco
            ctx.damage *= echoDamageMultiplier;

            enemy.ReceiveHit(
                ctx,
                owner.weaponMelee.stabilityBreak + owner.weaponRanged.stabilityBreak,
                owner.stabilityMultiplier.Current
            );

            owner.GetComponent<PlayerHealth>()?.TryApplyLifesteal();
        }

        Destroy(gameObject);
    }
}
