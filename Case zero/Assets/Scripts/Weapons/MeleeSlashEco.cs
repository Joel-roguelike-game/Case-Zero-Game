using UnityEngine;

/// <summary>
/// Melee exclusivo de los ecos
/// NO dispara eventos
/// NO genera loops
/// Replica EXACTAMENTE el melee del jugador pero desde el eco
/// </summary>
public class MeleeSlashEco : MonoBehaviour
{
    private PlayerStats sourcePlayer;
    private Transform echoTransform;
    private Vector2 direction;
    private float damageMultiplier;

    private float duration = 1f;
    private float timer;
    private bool hasHit;

    /// <summary>
    /// Inicializa el melee del eco
    /// </summary>
    public void Init(PlayerStats player, Transform echo, Vector2 dir, float dmgMult)
    {
        sourcePlayer = player;
        echoTransform = echo;
        direction = dir.normalized;
        damageMultiplier = dmgMult;

        SetupTransform();
    }

    private void SetupTransform()
    {
        // Rotación (idéntica al melee normal)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 45f);

        // Escala según rango melee del jugador
        float range = sourcePlayer.caCRange.Current*2;
        transform.localScale = new Vector3(range, range, 1f);

        // Posicionamiento desde el ECO (NO el jugador)
        Collider2D ecoCol = echoTransform.GetComponent<Collider2D>();
        Collider2D slashCol = GetComponent<Collider2D>();

        if (ecoCol && slashCol)
        {
            Vector2 edge = ecoCol.bounds.ClosestPoint(
                ecoCol.bounds.center + (Vector3)direction * 10f
            );

            float extent = Mathf.Max(
                slashCol.bounds.extents.x,
                slashCol.bounds.extents.y
            );

            transform.position = edge + direction * extent;
        }
        else
        {
            transform.position = echoTransform.position;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (hasHit) return;

        EnemyCombat enemy = col.GetComponent<EnemyCombat>();
        if (!enemy) return;

        hasHit = true;

        bool isParry = enemy.ConsumeParryAffected();

        DamageContext ctx = DamageCalculator.CalculatePlayerDamage(
            sourcePlayer,
            sourcePlayer.weaponMelee.flatDamage,
            sourcePlayer.weaponMelee.damagePercent,
            sourcePlayer.caCDmg.Current,
            sourcePlayer.critChance.Current,
            sourcePlayer.critDamage.Current,
            isParry,
            sourcePlayer.parryMultiplier.Current,
            enemy.GetComponent<EnemyStats>().stabilityBroken,
            sourcePlayer.stabilityMultiplier.Current,
            null
        );

        // +25% daño del eco
        ctx.damage *= damageMultiplier;

        enemy.ReceiveHit(
            ctx,
            sourcePlayer.weaponMelee.stabilityBreak,
            sourcePlayer.stabilityMultiplier.Current
        );

        sourcePlayer.GetComponent<PlayerHealth>()?.TryApplyLifesteal();
    }
}
