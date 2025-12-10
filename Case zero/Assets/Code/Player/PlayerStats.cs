using UnityEngine;

/*clase para las estadisticas del jugador*/
public class PlayerStats : MonoBehaviour
{
    public PlayerClassData clase;

    // Estadísticas
    public StatValue Hp;
    public StatValue Stamina;
    public StatValue CaCDmg;
    public StatValue DistDmg;
    public StatValue CritChance;
    public StatValue CritDamage;
    public StatValue MoveSpeed;
    public StatValue AtkSpeedCaC;
    public StatValue AtkSpeedDist;
    public StatValue ActualAmmo;

    // Modificadores
    public StatValue ParryMultiplier;
    public StatValue StabilityMultiplier;
    public StatValue DodgeSpeed;
    public StatValue StaminaRegen;
    public StatValue HpRegen;
    public StatValue LifestealPercent;
    public StatValue LifestealFlat;
    public StatValue CaCRange;

    // Progresión
    public int Nivel;
    public int XP;
    public int Oro;

    void Awake()
    {
        CargarClase(clase);
    }

    public void CargarClase(PlayerClassData data)
    {
        Hp = new StatValue(data.vida);
        Stamina = new StatValue(data.energia);
        CaCDmg = new StatValue(data.dañoCaC);
        DistDmg = new StatValue(data.dañoDist);
        CritChance = new StatValue(data.critChance);
        CritDamage = new StatValue(data.critDamage);
        MoveSpeed = new StatValue(data.moveSpeed);
        AtkSpeedCaC = new StatValue(data.atkSpeedCaC);
        AtkSpeedDist = new StatValue(data.atkSpeedDist);
        ActualAmmo = new StatValue(data.municionInicial);

        ParryMultiplier = new StatValue(data.parryMultiplier);
        StabilityMultiplier = new StatValue(data.estabilidadMultiplier);
        DodgeSpeed = new StatValue(data.dodgeSpeed);
        StaminaRegen = new StatValue(data.energiaRegen);
        HpRegen = new StatValue(data.saludRegen);
        LifestealPercent = new StatValue(data.lifestealPercent);
        LifestealFlat = new StatValue(data.lifestealFlat);
        CaCRange = new StatValue(data.alcanceCaC);

        Nivel = data.nivelInicial;
        XP = data.experienciaInicial;
        Oro = data.oroInicial;

        // Activar pasiva
        if (data.pasiva != null)
            data.pasiva.Activar(this);
    }
}
