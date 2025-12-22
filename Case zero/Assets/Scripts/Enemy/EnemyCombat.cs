using UnityEngine;
using System.Collections;

/*
 * EnemyCombat
 * 
 * Gestiona todo el combate del enemigo:
 * - Recepción de daño
 * - Sistema de estabilidad y rotura
 * - Daño recibido
 * - Daño por contacto (melee tipo bruiser)
 * - Interacción con el sistema de parry
 * - Gestión de textos de daño
 */
public class EnemyCombat : MonoBehaviour
{
    private EnemyStats stats;
    private EnemyHealth health;

    private DamageText activeDamageText;

    [Header("Stability")]
    public float stabilityRegenDelay = 5f;

    private Coroutine stabilityRoutine;
    private bool parryAffected;

    /*
     * Obtiene referencias a las estadísticas y a la vida del enemigo.
     */
    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        health = GetComponent<EnemyHealth>();
    }

    /*
     * Recibe un impacto desde el jugador:
     * - Aplica rotura de estabilidad
     * - Aplica daño
     * - Genera y acumula texto de daño
     */
    public void ReceiveHit(
        DamageResult result,
        float stabilityBreak,
        float playerStabilityMultiplier
    )
    {
        if (health == null || health.isDead)
            return;

        // === STABILITY ===
        if (!stats.stabilityBroken)
        {
            stats.currentStability -= stabilityBreak;
            if (stats.currentStability <= 0f)
            {
                stats.currentStability = 0f;
                stats.stabilityBroken = true;
            }
        }

        float finalDamage = result.damage;
        health.TakeDamage(finalDamage);

        int dmgInt = Mathf.FloorToInt(finalDamage);

        Vector3 textPos = transform.position + Vector3.up * 0.5f;

        if (activeDamageText == null)
        {
            activeDamageText = DamageTextSpawner.Instance.Spawn(
                textPos,
                dmgInt,
                result.isCrit
            );
        }
        else
        {
            activeDamageText.SetWorldPosition(textPos);

            activeDamageText.AddDamage(
                dmgInt,
                result.isCrit,
                DamageTextSpawner.Instance.config
            );
        }
    }

    /*
     * Limpia el texto de daño activo al destruir el enemigo.
     */
    private void OnDestroy()
    {
        if (activeDamageText != null)
            Destroy(activeDamageText.gameObject);
    }

    /*
     * Regenera completamente la estabilidad tras un tiempo de espera.
     */
    private IEnumerator RegenerateStability()
    {
        Debug.Log("Estabilidad rota, regenerando...");
        yield return new WaitForSeconds(stabilityRegenDelay);

        stats.currentStability = stats.baseStability;
        stats.stabilityBroken = false;
        Debug.Log("Estabilidad recuperada");
    }
    
    private bool canDealContactDamage = true;
    public float contactDamageCooldown = 1f;

    /*
     * Daño por contacto continuo mientras el jugador permanece en el collider.
     * Incluye cooldown para evitar daño constante cada frame.
     */
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!canDealContactDamage)
            return;

        if (!other.CompareTag("Player"))
            return;

        PlayerHealth ph = other.GetComponent<PlayerHealth>();
        if (ph == null)
            return;

        ph.TakeDamage(stats.baseDamage, this);
        StartCoroutine(ContactDamageCooldown());
    }

    /*
     * Cooldown del daño por contacto.
     */
    private IEnumerator ContactDamageCooldown()
    {
        canDealContactDamage = false;
        yield return new WaitForSeconds(contactDamageCooldown);
        canDealContactDamage = true;
    }

    /*
     * Marca al enemigo como afectado por un parry.
     */
    public void SetParryAffected()
    {
        parryAffected = true;
        Debug.Log("ENEMIGO AFECTADO POR PARRY");
    }

    /*
     * Consume el estado de parry en el siguiente golpe.
     */
    public bool ConsumeParryAffected()
    {
        if (!parryAffected)
            return false;

        parryAffected = false;
        Debug.Log("PARRY CONSUMIDO EN ESTE GOLPE");
        return true;
    }
}
