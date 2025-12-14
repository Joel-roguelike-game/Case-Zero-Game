using UnityEngine;

/*
 Representa un ataque cuerpo a cuerpo:
 - Se orienta hacia el ratón
 - Aplica daño y rotura de estabilidad
 - Vive un tiempo corto y desaparece
*/
public class MeleeSlash : MonoBehaviour
{
    [Header("Owner")]
    public PlayerStats owner;

    [Header("Attack Data")]
    public float damage;
    public float stabilityBreak;
    public float stabilityMultiplier;

    public Vector2 direction;

    [Header("Lifetime")]
    public float duration = 0.15f;
    private float timer;

    private void Start()
    {
        // Rotar hacia la dirección del ataque
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 45f);

        // Escalar según rango CaC del jugador
        float range = owner.caCRange.Current;
        transform.localScale = new Vector3(range, range, 1f);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
            Destroy(gameObject);
    }

    private bool hasHit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit)
            return;

        EnemyCombat enemy = collision.GetComponentInParent<EnemyCombat>();
        if (enemy == null)
            return;

        
        EnemyStats enemystats = collision.GetComponentInParent<EnemyStats>();
        if (enemystats == null)
            return;
        hasHit = true;

        bool wasCrit;
        float dmg = DamageCalculator.CalculatePlayerDamage(
            owner.weaponMelee.flatDamage,
            owner.weaponMelee.damagePercent,
            owner.caCDmg.Current,
            owner.critChance.Current,
            owner.critDamage.Current,
            false, // parry se meterá luego
            owner.parryMultiplier.Current,
            enemystats.stabilityBroken,
            owner.stabilityMultiplier.Current,
            out wasCrit
        );

        enemy.ReceiveHit(dmg, owner.weaponMelee.stabilityBreak, owner.stabilityMultiplier.Current);

        //DamageTextSpawner.Spawn(enemy.transform.position, dmg, wasCrit);
    }

}