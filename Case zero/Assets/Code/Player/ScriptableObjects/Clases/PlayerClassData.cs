using UnityEngine;

[CreateAssetMenu(fileName = "PlayerClass", menuName = "Ephidra/Player Class")]
/*gestor de datos de la clase*/
public class PlayerClassData : ScriptableObject
{
    [Header("Stats Base")]
    public float vida;
    public float energia;
    public float dañoCaC;
    public float dañoDist;
    public float critChance;
    public float critDamage;
    public float moveSpeed;
    public float atkSpeedCaC;
    public float atkSpeedDist;
    public int municionInicial;

    [Header("Modificadores Base")]
    public float parryMultiplier;
    public float estabilidadMultiplier;
    public float dodgeSpeed;
    public float energiaRegen;
    public float saludRegen;
    public float lifestealPercent;
    public float lifestealFlat;
    public float alcanceCaC;
    
    [Header("XP / Oro")]
    public int nivelInicial = 1;
    public int experienciaInicial = 0;
    public int oroInicial = 0;

    [Header("Pasiva")]
    public PlayerClassPassive pasiva;
}