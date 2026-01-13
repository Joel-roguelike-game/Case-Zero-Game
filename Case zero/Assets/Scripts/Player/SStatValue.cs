using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatValue
{
    public float Base;

    private Dictionary<object, float> flatBonuses =
        new Dictionary<object, float>();
    
    public float FlatBonus;
    public float PercentBonus;
    public float Multiplier = 1f;

    public float Current;

    public StatValue(float baseValue)
    {
        Base = baseValue;
        Recalculate();
    }

    /*
     * Recalcula el valor final aplicando todas las capas
     */
    public void Recalculate()
    {
        float value = Base;
        value += FlatBonus;
        value *= (1f + PercentBonus);
        value *= Mathf.Max(1f, Multiplier);

        Current = value;
    }

    public void AddFlat(float value)
    {
        FlatBonus += value;
        Recalculate();
    }

    public void AddPercent(float value)
    {
        PercentBonus += value;
        Recalculate();
    }

    public void AddMultiplier(float value)
    {
        Multiplier *= value;
        Recalculate();
    }

    public void ResetModifiers()
    {
        FlatBonus = 0f;
        PercentBonus = 0f;
        Multiplier = 1f;
        Recalculate();
    }
}