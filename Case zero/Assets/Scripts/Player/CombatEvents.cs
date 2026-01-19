using System;

public static class CombatEvents
{
    public static Action<PlayerStats, DamageContext> OnPlayerHit;
    public static Action<PlayerStats> OnParrySuccess;
    public static Action<PlayerStats> OnDash;
    public static Action<PlayerStats> OnDashEvade;
    public static Action<PlayerStats, EnemyCombat> OnEnemyKilled;
}