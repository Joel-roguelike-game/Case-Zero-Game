using UnityEngine;

public abstract class BaseDotInstance
{
    public float remainingDuration;
    protected float tickTimer;

    public abstract void Tick(EnemyCombat enemy);
    public abstract float Explode();
}