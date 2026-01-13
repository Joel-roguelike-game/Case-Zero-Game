using UnityEngine;

[CreateAssetMenu(fileName = "SOPlayerClass", menuName = "Classes/Player Class")]
/*Crea las estadisticas del jugador en un ScriptableObject*/
public class SOPlayerClass : ScriptableObject
{
    [Header("Stats Base")]
    public SOWeapon weaponMelee;
    public SOWeapon weaponRanged;

    [Header("Stats Base")]
    public float maxHP = 100f;
    public float maxStamina = 50f;
    public float caCDmg = 10f;
    public float distDmg = 10f;
    public float critChance = 10f;
    public float critDamage = 150f;
    public float moveSpeed = 10f;
    public float atkSpeedCaC = 1f;
    public float atkSpeedDist = 1f;
    public int actualAmmo = 20;

    [Header("Focus")]
    public float maxFocus = 100f;
    public float focusRegen = 0f;

    [Header("Modificadores Base")]
    public float parryMultiplier = 3f;
    public float stabilityMultiplier = 1.5f;
    public float dodgeSpeed = 1f;
    public float staminaRegen = 0f;
    public float hpRegen = 0f;
    public float lifestealPercent = 0f;
    public float lifestealFlat = 0f;
    public float caCRange = 1f;

    [Header("XP / Oro Base")]
    public int level = 1;
    public int xP = 0;
    public int gold = 0;

    public SOClassPassive passive;
    public SOClassActive active;
}