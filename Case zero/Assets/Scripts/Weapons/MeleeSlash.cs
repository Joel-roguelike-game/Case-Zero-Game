// MeleeSlash.cs
using UnityEngine;

public class MeleeSlash : MonoBehaviour
{
    public PlayerStats owner;
    public float stabilityBreak;
    public float stabilityMultiplier;
    public Vector2 direction;

    public float duration = 1f;
    private float timer;
    private bool hasHit;
    

    private void Start()
    {
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 45f);

        float range = owner.caCRange.Current;
        transform.localScale = new Vector3(range, range, 1f);

        Collider2D playerCol = owner.GetComponent<Collider2D>();
        Collider2D slashCol = GetComponent<Collider2D>();
        if (playerCol && slashCol)
        {
            Vector2 edge = playerCol.bounds.ClosestPoint(playerCol.bounds.center + (Vector3)direction * 10f);
            float extent = Mathf.Max(slashCol.bounds.extents.x, slashCol.bounds.extents.y);
            transform.position = edge + direction * extent;
        }

        // ⚠️ Solo disparar el evento si es un ataque de jugador
        /*if (owner != null && !this.CompareTag("EchoMelee"))
        {
            AttackContext ctx = new AttackContext(direction, AttackType.Melee, owner, AttackSource.Player);
            CombatEvents.OnPlayerAttack?.Invoke(owner, ctx);
        }*/
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
            owner,
            owner.weaponMelee.flatDamage,
            owner.weaponMelee.damagePercent,
            owner.caCDmg.Current,
            owner.critChance.Current,
            owner.critDamage.Current,
            isParry,
            owner.parryMultiplier.Current,
            enemy.GetComponent<EnemyStats>().stabilityBroken,
            owner.stabilityMultiplier.Current,
            null
        );

        enemy.ReceiveHit(ctx, stabilityBreak, owner.stabilityMultiplier.Current);
        owner.GetComponent<PlayerHealth>()?.TryApplyLifesteal();
    }
}
