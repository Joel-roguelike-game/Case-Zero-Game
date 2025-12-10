using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerStats : MonoBehaviour
{
    [Header("Clase (ScriptableObject base)")]
    public SOPlayerClass clase; // asignar el SO de la clase en el inspector o cargar en runtime

    // Estadísticas (Base = de SO, Current = en run)
    public StatValue hp;
    public StatValue stamina;      // energía actual (cap = MaxStamina)
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
    public int nivel;
    public float xP;       // XP actual
    public float maxHp;    //  Vida maxima actual
    public int oro;

    // Tope de stamina
    public float MaxStamina; // base para capacidad de stamina (se sincroniza con Stamina.Base en CargarClase)

    // Pasiva instanciada como child (opcional)
    private GameObject passiveInstance;

    private void Awake()
    {
        if (clase != null)
            CargarClase(clase);
    }

    /// <summary>
    /// Inicializa las StatValues y variables desde el ScriptableObject de la clase.
    /// Llamar al inicio de la run o al seleccionar clase.
    /// </summary>
    public void CargarClase(SOPlayerClass data)
    {
        if (data == null) return;

        clase = data;

        hp = new StatValue(data.vida);
        stamina = new StatValue(data.energia);
        caCDmg = new StatValue(data.dañoCaC);
        distDmg = new StatValue(data.dañoDist);
        critChance = new StatValue(data.critChance);
        critDamage = new StatValue(data.critDamage);
        moveSpeed = new StatValue(data.moveSpeed);
        atkSpeedCaC = new StatValue(data.atkSpeedCaC);
        atkSpeedDist = new StatValue(data.atkSpeedDist);
        actualAmmo = new StatValue(data.municionInicial);

        parryMultiplier = new StatValue(data.parryMultiplier);
        stabilityMultiplier = new StatValue(data.estabilidadMultiplier);
        dodgeSpeed = new StatValue(data.dodgeSpeed);
        staminaRegen = new StatValue(data.energiaRegen);
        hpRegen = new StatValue(data.saludRegen);
        lifestealPercent = new StatValue(data.lifestealPercent);
        lifestealFlat = new StatValue(data.lifestealFlat);
        caCRange = new StatValue(data.alcanceCaC);

        nivel = data.nivelInicial;
        xP = data.experienciaInicial;
        oro = data.oroInicial;

        maxHp = data.maxHp;
        MaxStamina = data.maxStamina;

        // sincroniza Stamina.Base con MaxStamina por si el SO lo define distinto
        stamina.Base = MaxStamina;
        stamina.Current = Mathf.Min(stamina.Current, stamina.Base);

        // Instanciar la pasiva (si existe)
        if (passiveInstance != null) Destroy(passiveInstance);
        if (data.pasivaPrefab != null)
        {
            passiveInstance = Instantiate(data.pasivaPrefab, transform);
            // la pasiva puede buscar PlayerStats en OnEnable !!!
        }
    }
}
