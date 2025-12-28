using System;
using UnityEngine;

/*
 * PlayerStats
 * 
 * Contiene todas las estadísticas del jugador.
 * Inicializa valores desde el ScriptableObject de la clase
 * y mantiene los valores runtime durante la partida.
 */
[RequireComponent(typeof(Collider2D))]
public class PlayerStats : MonoBehaviour
{
    [Header("Clase (ScriptableObject base)")]
    public SOPlayerClass pClass;

    public SOWeapon weaponMelee;
    public SOWeapon weaponRanged;

    // === STATS BASE ===
    public StatValue maxHP;
    public StatValue maxStamina;
    public StatValue caCDmg;
    public StatValue distDmg;
    public StatValue critChance;
    public StatValue critDamage;
    public StatValue moveSpeed;
    public StatValue atkSpeedCaC;
    public StatValue atkSpeedDist;
    public StatValue actualAmmo;

    public StatValue parryMultiplier;
    public StatValue stabilityMultiplier;
    public StatValue dodgeSpeed;
    public StatValue staminaRegen;
    public StatValue hpRegen;
    public StatValue lifestealPercent;
    public StatValue lifestealFlat;
    public StatValue caCRange;

    // === PROGRESIÓN ===
    public int level;
    public float xP;
    public int gold;

    // === RUNTIME ===
    public float currentHp;
    public float currentStamina;

    // Valores base runtime (ya con ítems, nivel, buffs PERMANENTES)
     public float baseCaCDmgRuntime;
     public float baseDistDmgRuntime;

    private float staminaRegenTimer;

    public SOClassPassive classPassive;
    public SOClassActive classActive;

    public float lastActiveTime = -999f;

    /*
     * Carga la clase asignada al iniciar.
     */
    private void Awake()
    {
        if (pClass != null)
            LoadClass(pClass);
    }

    /*
     * Inicializa todas las estadísticas a partir del ScriptableObject
     * de la clase del jugador.
     */
    public void LoadClass(SOPlayerClass data)
    {
        // Desactivar pasiva anterior
        if (classPassive != null)
            classPassive.Deactivate(this);

        pClass = data;

        weaponMelee = data.weaponMelee;
        weaponRanged = data.weaponRanged;

        // Inicialización de stats
        maxHP = new StatValue(data.maxHP);
        maxStamina = new StatValue(data.maxStamina);
        caCDmg = new StatValue(data.caCDmg);
        distDmg = new StatValue(data.distDmg);
        critChance = new StatValue(data.critChance);
        critDamage = new StatValue(data.critDamage);
        moveSpeed = new StatValue(data.moveSpeed);
        atkSpeedCaC = new StatValue(data.atkSpeedCaC);
        atkSpeedDist = new StatValue(data.atkSpeedDist);
        actualAmmo = new StatValue(data.actualAmmo);

        parryMultiplier = new StatValue(data.parryMultiplier);
        stabilityMultiplier = new StatValue(data.stabilityMultiplier);
        dodgeSpeed = new StatValue(data.dodgeSpeed);
        staminaRegen = new StatValue(data.staminaRegen);
        hpRegen = new StatValue(data.hpRegen);
        lifestealPercent = new StatValue(data.lifestealPercent);
        lifestealFlat = new StatValue(data.lifestealFlat);
        caCRange = new StatValue(data.caCRange);

        // Progresión
        level = data.level;
        xP = data.xP;
        gold = data.gold;

        // Vida / stamina inicial
        currentHp = maxHP.Base;
        currentStamina = maxStamina.Base;

        // Guardamos los valores BASE reales de daño (clave para Xeno)
        baseCaCDmgRuntime = caCDmg.Current;
        baseDistDmgRuntime = distDmg.Current;

        classPassive = data.passive;
        classActive = data.active;

        // Activar pasiva de clase
        if (classPassive != null)
            classPassive.Activate(this);
    }

    /*
     * Consume stamina si hay suficiente.
     * Devuelve true si el consumo fue exitoso.
     */
    public bool ConsumeStamina(float amount)
    {
        if (currentStamina < amount)
            return false;

        currentStamina -= amount;
        staminaRegenTimer = 0f;
        return true;
    }

    /*
     * Controla la regeneración de stamina.
     * Se llama desde Update si el jugador está en estado válido.
     */
    public void RegenerateStamina(bool canRegen)
    {
        if (!canRegen)
        {
            staminaRegenTimer = 0f;
            return;
        }

        staminaRegenTimer += Time.deltaTime;

        if (staminaRegenTimer >= 0.1f)
        {
            currentStamina += staminaRegen.Current;
            currentStamina = Mathf.Min(
                currentStamina,
                maxStamina.Current
            );

            staminaRegenTimer = 0f;
        }
    }

    /*
     * Intenta activar la habilidad activa de la clase
     * respetando el cooldown.
     */
    public void TryActivate()
    {
        if (classActive == null)
            return;

        if (Time.time < lastActiveTime + classActive.cooldown)
        {
            Debug.Log(
                "Habilidad en Cooldown: " +
                Time.time + "/" + (lastActiveTime + classActive.cooldown)
            );
            return;
        }

        classActive.Activar(this);
    }
}
