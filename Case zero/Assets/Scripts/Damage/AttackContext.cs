// AttackContext.cs
using UnityEngine;

public struct AttackContext
{
    public Vector2 direction;
    public AttackType type;
    public AttackSource source;
    public PlayerStats owner;

    public AttackContext(Vector2 direction, AttackType type, PlayerStats owner, AttackSource source)
    {
        this.direction = direction;
        this.type = type;
        this.owner = owner;
        this.source = source;
    }
}

public enum AttackType
{
    Melee,
    Ranged
}

public enum AttackSource
{
    Player,
    Echo
}