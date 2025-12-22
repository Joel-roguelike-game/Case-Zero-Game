using UnityEngine;

/*
 * EnemyStats
 *
 * Contiene todas las estadísticas del enemigo:
 * - Vida
 * - Daño
 * - Estabilidad
 * - Velocidad
 * Incluye escalado por nivel y valores runtime.
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

    /*
     * Aplica el escalado inicial por nivel
     * y configura los valores runtime.
     */
    private void Awake()
    {
        ApplyLevelScaling();
        currentHealth = baseHealth;
        currentStability = baseStability;
    }

    /*
     * Escala las estadísticas base según el nivel del enemigo.
     */
    private void ApplyLevelScaling()
    {
        baseHealth *= 1f + (level - 1) * 0.3f;
        baseDamage *= 1f + (level - 1) * 0.25f;
        baseStability *= 1f + (level - 1) * 0.2f;
    }
    
    /*
     * Inicializa las estadísticas del enemigo a partir de un nivel dado.
     * Usado principalmente por enemigos invocados.
     */
    public void InitializeFromLevel(int lvl)
    {
        level = lvl;
        ApplyLevelScaling();
        currentHealth = baseHealth;
        currentStability = baseStability;
    }
}