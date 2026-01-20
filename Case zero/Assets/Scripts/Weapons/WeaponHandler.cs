using UnityEngine;
using UnityEngine.InputSystem;

/*
 * WeaponHandler
 *
 * Gestiona el uso de armas del jugador:
 * - Input
 * - Cadencia real
 * - Spawneo de ataques
 *
 * NO calcula daño.
 */
public class WeaponHandler : MonoBehaviour
{
    private PlayerStats stats;
    private Collider2D playerCollider;
    private Camera cam;
    private PlayerInputController inputController;

    private float nextAttackTime = 0f;
    public bool isAttacking;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        playerCollider = GetComponent<Collider2D>();
        inputController = GetComponent<PlayerInputController>();
        cam = Camera.main;
    }

    private void Update()
    {
        isAttacking = false;

        if (Time.time < nextAttackTime)
            return;

        bool meleeHeld =
            inputController.inputActions.Gameplay.MeleeAttack.ReadValue<float>() > 0.1f;

        bool rangedHeld =
            inputController.inputActions.Gameplay.RangedAttack.ReadValue<float>() > 0.1f;

        if (meleeHeld && stats.weaponMelee != null)
        {
            if (!stats.ConsumeStamina(10f))
                return;

            UseMelee();
            nextAttackTime = Time.time + GetMeleeCooldown();
            isAttacking = true;
            return;
        }

        if (rangedHeld && stats.weaponRanged != null)
        {
            UseRanged();
            nextAttackTime = Time.time + GetRangedCooldown();
            isAttacking = true;
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

    /*
     * Ataque cuerpo a cuerpo
     */
    private void UseMelee()
    {
        Vector2 dir = GetMouseDirection();
        float radius =
            playerCollider.bounds.extents.magnitude * stats.caCRange.Current;

        Vector2 pos =
            (Vector2)playerCollider.bounds.center + dir * radius;

        GameObject slashObj = Instantiate(
            stats.weaponMelee.weaponPrefab,
            pos,
            Quaternion.identity
        );

        MeleeSlash slash = slashObj.GetComponent<MeleeSlash>();
        slash.owner = stats;
        slash.direction = dir;
        slash.stabilityBreak = stats.weaponMelee.stabilityBreak;
        slash.stabilityMultiplier = stats.stabilityMultiplier.Current;

        Collider2D slashCol = slashObj.GetComponent<Collider2D>();
        if (slashCol != null)
            Physics2D.IgnoreCollision(playerCollider, slashCol);

        //Debug.Log("[WeaponHandler] Melee attack spawned");
    }

    /*
     * Ataque a distancia
     */
    private void UseRanged()
    {
        if (stats.currentAmmo <= 0)
            return;

        Vector2 baseDir = GetMouseDirection();

        if (stats.weaponRanged.weaponName == "Escopeta")
        {
            ShootShotgun(baseDir);
        }
        else
        {
            Vector2 dir =
                ApplySpread(baseDir, GetSpread(stats.weaponRanged));

            ShootProjectile(dir);
        }

        stats.currentAmmo--;
    }

    private float GetSpread(SOWeapon weapon)
    {
        if (weapon.weaponName == "Ballesta" || weapon.weaponName == "Rifle")
            return 0f;

        return weapon.weaponName == "Escopeta" ? 25f : 10f;
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
            Vector2 dir = ApplySpread(baseDir, 25f);
            ShootProjectile(dir);
        }
    }

    private void ShootProjectile(Vector2 direction)
    {
        Vector2 firePoint = GetDynamicFirePoint(direction);

        GameObject projObj = Instantiate(
            stats.weaponRanged.weaponPrefab,
            firePoint,
            Quaternion.identity
        );

        Projectile p = projObj.GetComponent<Projectile>();
        p.direction = direction;
        p.owner = stats;

        //Debug.Log("[WeaponHandler] Projectile spawned");
    }
}
