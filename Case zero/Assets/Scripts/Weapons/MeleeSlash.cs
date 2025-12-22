using UnityEngine;

/*
 Representa un ataque cuerpo a cuerpo del jugador.
 Se instancia como una hitbox temporal orientada hacia el ratón.
 Aplica daño, rotura de estabilidad y tiene una vida muy corta.
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

    /*
     * Inicializa el ataque:
     * - Rota el slash según la dirección
     * - Escala el tamaño según el rango CaC del jugador
     */
    private void Start()
    {
        // Rotar hacia la dirección del ataque
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 45f);

        // Escalar según rango CaC del jugador
        float range = owner.caCRange.Current;
        transform.localScale = new Vector3(range, range, 1f);
    }

    /*
     * Controla la duración del ataque.
     * Al terminar su tiempo de vida, se destruye.
     */
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
            Destroy(gameObject);
    }

    private bool hasHit;

    /*
     * Detecta colisión con enemigos.
     * Calcula el daño final y lo envía al sistema de combate enemigo.
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyCombat enemy = collision.GetComponent<EnemyCombat>();
        if (enemy == null)
            return;

        bool isParry = enemy.ConsumeParryAffected();

        DamageResult result = DamageCalculator.CalculatePlayerDamage(
            owner.weaponMelee.flatDamage,
            owner.weaponMelee.damagePercent,
            owner.caCDmg.Current,
            owner.critChance.Current,
            owner.critDamage.Current,
            isParry,
            owner.parryMultiplier.Current,
            enemy.GetComponent<EnemyStats>().stabilityBroken,
            owner.stabilityMultiplier.Current
        );

        enemy.ReceiveHit(
            result,
            stabilityBreak,
            owner.stabilityMultiplier.Current
        );
    }
}
