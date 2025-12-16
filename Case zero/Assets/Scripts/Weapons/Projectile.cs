using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 100f;
    public Vector2 direction;
    public float lifeTime = 3f;

    public float damageMultiplier = 1f;
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
        if (hasHit)
            return;

        EnemyCombat enemy = other.GetComponentInParent<EnemyCombat>();
        if (enemy == null)
            return;

        hasHit = true;

        DamageResult result = DamageCalculator.CalculatePlayerDamage(
            owner.weaponRanged.flatDamage,
            owner.weaponRanged.damagePercent,
            owner.distDmg.Current,
            owner.critChance.Current,
            owner.critDamage.Current,
            false, // parry
            owner.parryMultiplier.Current,
            enemy.GetComponent<EnemyStats>().stabilityBroken,
            owner.stabilityMultiplier.Current
        );

        enemy.ReceiveHit(
            result,
            owner.weaponRanged.stabilityBreak,
            owner.stabilityMultiplier.Current
        );

        Destroy(gameObject);
    }
}