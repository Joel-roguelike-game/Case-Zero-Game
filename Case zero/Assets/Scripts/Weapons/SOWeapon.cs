using UnityEngine;

/*
 Define un arma como ScriptableObject.
 Contiene estadísticas base, categoría, datos de evolución
 y referencias a prefabs y pasivas.
*/
public enum WeaponCategory { Melee, Ranged }

[CreateAssetMenu(fileName = "SOWeapon", menuName = "Weapons/Weapon")]
public class SOWeapon : ScriptableObject
{
    [Header("Identidad del arma")]
    public string weaponName;
    public WeaponCategory category;
    public bool isEvolution;

    [Header("Estadísticas base del arma")]
    public float damagePercent = 100f;
    public float flatDamage = 5f;
    public float stabilityBreak = 10f;
    public float attackSpeed = 1f;
    public int maxAmmo = 0;

    [Header("Prefab del arma")]
    public GameObject weaponPrefab;

    [Header("Opciones de evolución")]
    public SOWeapon evolveOptionA;
    public SOWeapon evolveOptionB;

    [Header("Datos de evolución")]
    public SOWeapon componentA;
    public SOWeapon componentB;
    public GameObject evolutionPassivePrefab;
}