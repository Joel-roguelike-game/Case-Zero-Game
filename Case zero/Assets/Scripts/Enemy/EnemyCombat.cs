using UnityEngine;
using System.Collections;

/*
 Gestiona:
 - Recepción de daño
 - Rotura de estabilidad
 - Daño aumentado durante rotura
 - Ataque del Bruiser
 - gestiona el estado de parry del enemigo
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


    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        health = GetComponent<EnemyHealth>();
    }

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

    private void OnDestroy()
    {
        if (activeDamageText != null)
            Destroy(activeDamageText.gameObject);
    }


    /*
     Regenera la estabilidad tras 5 segundos completamente.
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
        ph.TakeDamage(stats.baseDamage, this);
        StartCoroutine(ContactDamageCooldown());
    }

    private IEnumerator ContactDamageCooldown()
    {
        canDealContactDamage = false;
        yield return new WaitForSeconds(contactDamageCooldown);
        canDealContactDamage = true;
    }
    //logica para el parry
    public void SetParryAffected()
    {
        parryAffected = true;
        Debug.Log("ENEMIGO AFECTADO POR PARRY");
    }

    public bool ConsumeParryAffected()
    {
        if (!parryAffected)
            return false;

        parryAffected = false;
        Debug.Log("PARRY CONSUMIDO EN ESTE GOLPE");
        return true;
    }



}