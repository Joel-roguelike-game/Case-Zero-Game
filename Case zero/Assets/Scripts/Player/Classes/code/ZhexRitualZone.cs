using UnityEngine;

public class ZhexRitualZone : MonoBehaviour
{
    public float duration = 6f;
    private float timer;
    private PlayerStats owner;

    public void Init(PlayerStats stats)
    {
        owner = stats;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            ApplyFinalDot();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyCombat enemy = other.GetComponentInParent<EnemyCombat>();
        if (enemy) ApplyDot(enemy);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        EnemyCombat enemy = other.GetComponentInParent<EnemyCombat>();
        if (!enemy) return;

        EnemyDotController dots = enemy.GetComponent<EnemyDotController>();
        if (!dots) return;

        float dmg = dots.ExplodeAllDots(0.5f);

        DamageContext ctx = DamageCalculator.CreatePureDamage(
            owner,
            dmg,
            false
        );

        enemy.ReceiveHit(ctx, 0f, 1f);
        ApplyDot(enemy);
    }

    private void ApplyDot(EnemyCombat enemy)
    {
        EnemyDotController dots = enemy.GetComponent<EnemyDotController>();
        if (!dots) return;

        float dotDamage =
            (owner.weaponRanged.damagePercent / 3f) *
            owner.distDmg.Current;

        dots.AddDot(new ZhexBloodDot(owner, dotDamage, 8f));
    }

    private void ApplyFinalDot()
    {
        foreach (EnemyCombat enemy in FindObjectsOfType<EnemyCombat>())
            ApplyDot(enemy);
    }
}