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
        EnemyHealth eh = other.GetComponent<EnemyHealth>();
        if (eh == null || eh.isDead)
            return; //control para muertos o nulos.

        if (hasHit)
            return;

        EnemyCombat enemy = other.GetComponentInParent<EnemyCombat>();
        if (enemy == null)
            return;

        hasHit = true;

        bool isParry = enemy.ConsumeParryAffected();

        DamageResult result = DamageCalculator.CalculatePlayerDamage(
            owner.weaponRanged.flatDamage,
            owner.weaponRanged.damagePercent,
            owner.distDmg.Current,
            owner.critChance.Current,
            owner.critDamage.Current,
            isParry,
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