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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyCombat enemy = collision.GetComponent<EnemyCombat>();
        if (enemy == null)
            return;

        enemy.ReceiveHit(
            damage,
            stabilityBreak,
            stabilityMultiplier
        );
    }
}