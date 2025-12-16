using UnityEngine;
using System.Collections;

/*
 Gestiona:
 - Recepción de daño
 - Rotura de estabilidad
 - Daño aumentado durante rotura
 - Ataque del Bruiser
*/
public class EnemyCombat : MonoBehaviour
{
    private EnemyStats stats;
    private EnemyHealth health;

    [Header("Stability")]
    public float stabilityRegenDelay = 5f;

    private Coroutine stabilityRoutine;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        health = GetComponent<EnemyHealth>();
    }

    /*
     Llamado cuando el enemigo recibe un golpe del jugador.
    */
    public void ReceiveHit(
        DamageResult result,
        float stabilityBreak,
        float playerStabilityMultiplier
    )
    {
        // -------- STABILITY --------
        if (!stats.stabilityBroken)
        {
            stats.currentStability -= stabilityBreak;

            if (stats.currentStability <= 0f)
            {
                stats.currentStability = 0f;
                stats.stabilityBroken = true;

                if (stabilityRoutine != null)
                    StopCoroutine(stabilityRoutine);

                stabilityRoutine = StartCoroutine(RegenerateStability());
            }
        }

        // -------- DAMAGE --------
        float finalDamage = stats.stabilityBroken
            ? result.damage * playerStabilityMultiplier
            : result.damage;

        health.TakeDamage(finalDamage);

        // -------- FEEDBACK --------
        DamageTextSpawner.Instance.Spawn(
            transform.position + Vector3.up * 0.5f,
            finalDamage,
            result.isCrit
        );
    }


    /*
     Regenera la estabilidad tras 5 segundos completamente.
    */
    private IEnumerator RegenerateStability()
    {
        yield return new WaitForSeconds(stabilityRegenDelay);

        stats.currentStability = stats.baseStability;
        stats.stabilityBroken = false;
    }
    
    private bool canDealContactDamage = true;
    public float contactDamageCooldown = 1f;

    /*
     * intenta dañar mientras este dentro.
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

        ph.TakeDamage(stats.baseDamage);
        StartCoroutine(ContactDamageCooldown());
    }

    private IEnumerator ContactDamageCooldown()
    {
        canDealContactDamage = false;
        yield return new WaitForSeconds(contactDamageCooldown);
        canDealContactDamage = true;
    }



}