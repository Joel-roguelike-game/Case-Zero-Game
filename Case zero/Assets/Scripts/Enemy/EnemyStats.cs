using UnityEngine;

/*
 Estadísticas del enemigo.
 Escalan con el nivel.
 Incluye sistema de estabilidad integrado.
*/
public class EnemyStats : MonoBehaviour
{
    [Header("Level")]
    public int level = 1;

    [Header("Base Stats")]
    public float baseHealth = 100f;
    public float baseDamage = 10f;
    public float baseStability = 50f;
    public float moveSpeed = 2f;

    [Header("Runtime")]
    public float currentHealth;
    public float currentStability;

    [Header("Stability State")]
    public bool stabilityBroken;

    private void Awake()
    {
        ApplyLevelScaling();
        currentHealth = baseHealth;
        currentStability = baseStability;
    }

    /*
     Escala las estadísticas según el nivel del enemigo.
    */
    private void ApplyLevelScaling()
    {
        baseHealth *= 1f + (level - 1) * 0.3f;
        baseDamage *= 1f + (level - 1) * 0.25f;
        baseStability *= 1f + (level - 1) * 0.2f;
    }
}
