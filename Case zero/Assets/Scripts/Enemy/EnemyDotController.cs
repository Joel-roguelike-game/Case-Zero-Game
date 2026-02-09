using System.Collections.Generic;
using UnityEngine;

public class EnemyDotController : MonoBehaviour
{
    private List<BaseDotInstance> dots = new();

    private EnemyCombat enemy;

    private void Awake()
    {
        enemy = GetComponent<EnemyCombat>();
    }

    private void Update()
    {
        for (int i = dots.Count - 1; i >= 0; i--)
        {
            dots[i].Tick(enemy);

            if (dots[i].remainingDuration <= 0)
                dots.RemoveAt(i);
        }
    }

    public void AddDot(BaseDotInstance dot)
    {
        dots.Add(dot);
    }

    public float ExplodeAllDots(float multiplier = 1f)
    {
        float total = 0f;

        foreach (var dot in dots)
            total += dot.Explode() * multiplier;

        dots.Clear();
        return total;
    }

    public int CountTotalRemainingSeconds(int maxPerDot)
    {
        int total = 0;

        foreach (var dot in dots)
            total += Mathf.Min(maxPerDot, Mathf.CeilToInt(dot.remainingDuration));

        return total;
    }
}