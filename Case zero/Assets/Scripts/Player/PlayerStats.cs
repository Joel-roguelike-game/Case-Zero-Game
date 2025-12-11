using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerStats : MonoBehaviour
{
    [Header("Clase (ScriptableObject base)")]
    public SOPlayerClass pClass; // asignar el SO de la clase en el inspector o cargar en runtime

    // Estadísticas (Base = de SO, Current = en run)
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

    // Modificadores
    public StatValue parryMultiplier;
    public StatValue stabilityMultiplier;
    public StatValue dodgeSpeed;
    public StatValue staminaRegen;
    public StatValue hpRegen;
    public StatValue lifestealPercent;
    public StatValue lifestealFlat;
    public StatValue caCRange;

    // Progresión
    public int level;
    public float xP;       // XP actual
    public float currentHp;    //  Vida  actual
    public int gold;
    public float currentStamina; // stamina actual 

    // Pasiva instanciada como child (opcional)
    private GameObject passiveInstance;

    private void Awake()
    {
        if (pClass != null)
            LoadClass(pClass);
    }

    /// <summary>
    /// Inicializa las StatValues y variables desde el ScriptableObject de la clase.
    /// Llamar al inicio de la run o al seleccionar clase.
    /// </summary>
    public void LoadClass(SOPlayerClass data)
    {
        pClass = data;

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

        level = data.level;
        xP = data.xP;
        gold = data.gold;

        // HP y stamina siempre empiezan al máximo del valor actual
        currentHp = maxHP.Base;
        currentStamina = maxStamina.Base;

        // Instanciar pasiva
        if (passiveInstance != null) Destroy(passiveInstance);
        if (data.pasivaPrefab != null)
            passiveInstance = Instantiate(data.pasivaPrefab, transform);
    }
}
