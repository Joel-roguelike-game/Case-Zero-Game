using UnityEngine;

public enum WeaponCategory { Melee, Ranged }

[CreateAssetMenu(fileName = "SOWeapon", menuName = "Weapons/Weapon")]
public class SOWeapon : ScriptableObject
{
    [Header("Identidad del arma")]
    [Tooltip("Nombre del arma")]
    public string weaponName;

    [Tooltip("cuerpo a cuerpo o a distancia")]
    public WeaponCategory category;

    [Tooltip("true si esta arma es una evolución")]
    public bool isEvolution;

    [Header("Estadísticas base del arma")]
    [Tooltip("multiplica al current dmg del jugador y saca el daño real")]
    public float damagePercent = 100f;

    [Tooltip("Daño plano mínimo que siempre inflige el arma")]
    public float flatDamage = 5f;
    
    [Tooltip("Cantidad fija de estabilidad que rompe esta arma")]
    public float stabilityBreak = 10f;
    
    [Tooltip("Velocidad de atq por segundo")]
    public float attackSpeed = 1f;

    [Tooltip("Munición máxima (solo a distancia)")]
    public int maxAmmo = 0;

    [Header("Prefab del arma (hitbox, proyectil, animación…)")]
    public GameObject weaponPrefab;

    [Header("Opciones de evolución (solo para armas base)")]
    [Tooltip("Primera opción de evolución ")]
    public SOWeapon evolveOptionA;

    [Tooltip("Segunda opción de evolución ")]
    public SOWeapon evolveOptionB;

    [Header("Datos de evolución (solo si es evolucion)")]
    [Tooltip("Primer arma que puede evolucionar hacia esta arma")]
    public SOWeapon componentA;

    [Tooltip("Segunda arma que puede evolucionar hacia esta arma")]
    public SOWeapon componentB;

    [Tooltip("Prefab que contiene la pasiva única del arma evolucionada")]
    public GameObject evolutionPassivePrefab;
}