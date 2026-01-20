using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * EnemyCombat
 *
 * Gestiona todo el combate del enemigo:
 * - Recepción de daño
 * - Sistema de estabilidad
 * - Daño por contacto
 * - Interacción con parry
 * - Textos de daño
 */
public class EnemyCombat : MonoBehaviour
{
    private EnemyStats stats;
    public EnemyHealth health;

    private DamageText activeDamageText;
    private Dictionary<object, float> damageTakenModifiers =
        new Dictionary<object, float>();
    [Header("Stability")]
    public float stabilityRegenDelay = 5f;
    private Coroutine stabilityRoutine;

    private bool parryAffected;

    [Header("Contact Damage")]
    public float contactDamageCooldown = 1f;
    private bool canDealContactDamage = true;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        health = GetComponent<EnemyHealth>();
    }

    public void AddDamageTakenModifier(object source, float percent)
    {
        damageTakenModifiers[source] = percent;
    }
    
    public void RemoveDamageTakenModifier(object source)
    {
        damageTakenModifiers.Remove(source);
    }
    
    private float GetDamageTakenMultiplier()
    {
        if (damageTakenModifiers.Count == 0)
            return 1f;

        float m = 1f;
        foreach (float p in damageTakenModifiers.Values)
            m *= (1f + p);

        return m;
    }
    /*
     * Recibe un golpe del jugador
     */
    public void ReceiveHit(
        DamageContext ctx,
        float stabilityBreak,
        float playerStabilityMultiplier
    )
    {
        if (health == null || health.isDead)
            return;

        HandleStability(stabilityBreak);

        float damageMultiplier = GetDamageTakenMultiplier();
        float finalDamage = ctx.damage * damageMultiplier;

        // 🔧 Clonamos el contexto para no modificar el original
        DamageContext finalCtx = ctx;
        finalCtx.damage = finalDamage;

        health.TakeDamage(finalCtx);
        SpawnDamageText(finalDamage, ctx.isCrit);

        Debug.Log(
            $"[EnemyCombat] Damage received: {ctx.damage} → {finalDamage} " +
            $"(x{damageMultiplier:F2})"
        );
    }


    /*
     * Manejo de estabilidad
     */
    private void HandleStability(float stabilityBreak)
    {
        if (stats.stabilityBroken)
            return;

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

    private IEnumerator RegenerateStability()
    {
        yield return new WaitForSeconds(stabilityRegenDelay);

        stats.currentStability = stats.baseStability;
        stats.stabilityBroken = false;
    }

    /*
     * Daño por contacto
     */
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!canDealContactDamage)
            return;

        if (!other.CompareTag("Player"))
            return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null)
            return;

        playerHealth.TakeDamage(stats.baseDamage, this);
        StartCoroutine(ContactDamageCooldown());
    }

    private IEnumerator ContactDamageCooldown()
    {
        canDealContactDamage = false;
        yield return new WaitForSeconds(contactDamageCooldown);
        canDealContactDamage = true;
    }

    /*
     * Parry
     */
    public void SetParryAffected()
    {
        parryAffected = true;
    }

    public bool ConsumeParryAffected()
    {
        if (!parryAffected)
            return false;

        parryAffected = false;
        return true;
    }

    /*
     * Textos de daño
     */
    private void SpawnDamageText(float damage, bool isCrit)
    {
        int dmgInt = Mathf.FloorToInt(damage);
        Vector3 pos = transform.position + Vector3.up * 0.5f;

        if (activeDamageText == null)
        {
            activeDamageText = DamageTextSpawner.Instance.Spawn(
                pos,
                dmgInt,
                isCrit
            );
        }
        else
        {
            activeDamageText.SetWorldPosition(pos);
            activeDamageText.AddDamage(
                dmgInt,
                isCrit,
                DamageTextSpawner.Instance.config
            );
        }
    }

    private void OnDestroy()
    {
        if (activeDamageText != null)
            Destroy(activeDamageText.gameObject);
    }
}
