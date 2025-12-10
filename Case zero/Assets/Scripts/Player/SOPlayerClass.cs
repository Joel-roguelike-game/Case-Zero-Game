using UnityEngine;

[CreateAssetMenu(fileName = "SOPlayerClass", menuName = "Classes/Player Class")]
public class SOPlayerClass : ScriptableObject
{
    [Header("Stats Base")]
    public float vida = 100f;
    public float energia = 50f; // stamina base
    public float dañoCaC = 10f;
    public float dañoDist = 10f;
    public float critChance = 10f;
    public float critDamage = 150f;
    public float moveSpeed = 10f;
    public float atkSpeedCaC = 1f;
    public float atkSpeedDist = 1f;
    public int municionInicial = 20;

    [Header("Modificadores Base")]
    public float parryMultiplier = 3f;
    public float estabilidadMultiplier = 1.5f;
    public float dodgeSpeed = 1f;
    public float energiaRegen = 0f;
    public float saludRegen = 0f;
    public float lifestealPercent = 0f;
    public float lifestealFlat = 0f;
    public float alcanceCaC = 1f;

    [Header("XP / Oro")]
    public int nivelInicial = 1;
    public int experienciaInicial = 0;
    public int oroInicial = 0;
    public float maxHp = 100f;        // Vida maxima actual
    public float maxStamina = 50f;    // cap de Stamina para separar del actual

    [Header("Pasiva")]
    public GameObject pasivaPrefab; // prefab con el MonoBehaviour de la pasiva
}