using UnityEngine;
using UnityEngine.InputSystem;

/*
 Gestiona ataques melee y ranged del jugador.
 - Ataque continuo manteniendo pulsado
 - Cadencia REAL basada en stats
 - Sin disparos dobles (Time.time)
 - Melee y ranged mutuamente excluyentes
*/
public class WeaponHandler : MonoBehaviour
{
    private PlayerStats stats;
    private Collider2D playerCollider;
    private Camera cam;
    private PlayerInputController inputController;

    private float nextAttackTime = 0f;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        playerCollider = GetComponent<Collider2D>();
        inputController = GetComponent<PlayerInputController>();
        cam = Camera.main;
    }

    private void Update()
    {
        if (Time.time < nextAttackTime)
            return;

        bool meleeHeld  = inputController.inputActions.Gameplay.MeleeAttack.ReadValue<float>() > 0.1f;
        bool rangedHeld = inputController.inputActions.Gameplay.RangedAttack.ReadValue<float>() > 0.1f;

        // PRIORIDAD: MELEE
        if (meleeHeld && stats.weaponMelee != null)
        {
            UseMelee();
            nextAttackTime = Time.time + GetMeleeCooldown();
            return;
        }

        // RANGED
        if (rangedHeld && stats.weaponRanged != null)
        {
            UseRanged();
            nextAttackTime = Time.time + GetRangedCooldown();
        }
    }

    private float GetMeleeCooldown()
    {
        return 1f / (stats.weaponMelee.attackSpeed * stats.atkSpeedCaC.Current);
    }

    private float GetRangedCooldown()
    {
        return 1f / (stats.weaponRanged.attackSpeed * stats.atkSpeedDist.Current);
    }

    private Vector2 GetMouseDirection()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);
        return (mouseWorldPos - (Vector2)transform.position).normalized;
    }

    private Vector2 GetDynamicFirePoint(Vector2 direction)
    {
        float radius = playerCollider.bounds.extents.magnitude;
        return (Vector2)playerCollider.bounds.center + direction * radius;
    }

    // ================= MELEE =================

    public void UseMelee()
    {
        Vector2 dir = GetMouseDirection();
        float radius = playerCollider.bounds.extents.magnitude * stats.caCRange.Current;
        Vector2 pos = (Vector2)playerCollider.bounds.center + dir * radius;

        GameObject slashObj = Instantiate(
            stats.weaponMelee.weaponPrefab,
            pos,
            Quaternion.identity
        );

        MeleeSlash slash = slashObj.GetComponent<MeleeSlash>();
        slash.owner = stats;
        slash.direction = dir;

        Collider2D slashCol = slashObj.GetComponent<Collider2D>();
        if (slashCol != null)
            Physics2D.IgnoreCollision(playerCollider, slashCol);
    }

    // ================= RANGED =================

    public void UseRanged()
    {
        if (stats.actualAmmo.Current <= 0)
            return;

        Vector2 baseDir = GetMouseDirection();

        if (stats.weaponRanged.weaponName == "Escopeta")
        {
            ShootShotgun(baseDir);
        }
        else
        {
            Vector2 dir = ApplySpread(baseDir, GetSpread(stats.weaponRanged));
            ShootProjectile(stats.weaponRanged, dir, 1f);
        }

        stats.actualAmmo.Current--;
    }

    private float GetSpread(SOWeapon weapon)
    {
        if (weapon.weaponName == "Ballesta" || weapon.weaponName == "Rifle")
            return 0f;

        return weapon.weaponName == "Escopeta" ? 45f : 15f;
    }

    private Vector2 ApplySpread(Vector2 dir, float spread)
    {
        float angle = Random.Range(-spread * 0.5f, spread * 0.5f);
        return Quaternion.Euler(0, 0, angle) * dir;
    }

    private void ShootShotgun(Vector2 baseDir)
    {
        for (int i = 0; i < 5; i++)
        {
            Vector2 dir = ApplySpread(baseDir, 45f);
            ShootProjectile(stats.weaponRanged, dir, 0.2f);
        }
    }

    private void ShootProjectile(SOWeapon weapon, Vector2 direction, float dmgMultiplier)
    {
        Vector2 firePoint = GetDynamicFirePoint(direction);

        GameObject projObj = Instantiate(
            weapon.weaponPrefab,
            firePoint,
            Quaternion.identity
        );

        Projectile p = projObj.GetComponent<Projectile>();
        p.direction = direction;
        p.damageMultiplier = dmgMultiplier;
        p.owner = stats;
    }
}
