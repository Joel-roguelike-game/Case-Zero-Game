using UnityEngine;
using UnityEngine.InputSystem;

/*
 Gestiona ataques melee y ranged del jugador.
 Calcula firePoint dinámico según el borde del collider y la dirección del ratón.
*/ 
public class WeaponHandler : MonoBehaviour
{
    private PlayerStats stats;
    private Collider2D playerCollider;
    private Camera cam;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        playerCollider = GetComponent<Collider2D>();
        cam = Camera.main;
    }

    
    // Devuelve la dirección normalizada hacia el ratón.
    private Vector2 GetMouseDirection()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue(); // <-- NUEVA LÍNEA
        Vector2 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);
        return (mouseWorldPos - (Vector2)transform.position).normalized;
    }


    // Calcula el punto de salida en el borde exterior del jugador.
    private Vector2 GetDynamicFirePoint(Vector2 direction)
    {
        float radius = playerCollider.bounds.extents.magnitude;
        return (Vector2)playerCollider.bounds.center + direction * radius;
    }

    // atq CaC
    public void UseMelee()
    {
        if (stats.weaponMelee == null) return;

        Vector2 dir = GetMouseDirection();

        // el fire point depende del alcance CaCRange
        float radius = playerCollider.bounds.extents.magnitude * stats.caCRange.Current;
        Vector2 slashPos = (Vector2)playerCollider.bounds.center + dir * radius;

        GameObject slashObj = Instantiate(
            stats.weaponMelee.weaponPrefab,
            slashPos,
            Quaternion.identity
        );

        MeleeSlash slash = slashObj.GetComponent<MeleeSlash>();
        slash.owner = stats;
        slash.direction = dir;
    }

    // atq a dist
    public void UseRanged()
    {
        if (stats.weaponRanged == null) return;
        if (stats.actualAmmo.Current <= 0) return;

        Vector2 baseDir = GetMouseDirection();

        // aplicar spread según arma
        float spread = GetSpread(stats.weaponRanged);

        // shotgun (5 proyectiles)
        if (stats.weaponRanged.weaponName == "Escopeta")
        {
            ShootShotgun(baseDir);
        }
        else
        {
            Vector2 dirFinal = ApplySpread(baseDir, spread);
            ShootProjectile(stats.weaponRanged, dirFinal, 1f);
        }

        stats.actualAmmo.Current--; // gastar munición
    }

    private float GetSpread(SOWeapon weapon)
    {
        if (weapon.weaponName == "Ballesta") return 0f;
        if (weapon.weaponName == "Rifle") return 0f;
        return 15f;
    }

    private Vector2 ApplySpread(Vector2 dir, float spread)
    {
        float angle = Random.Range(-(spread * 0.5f), (spread * 0.5f));
        return Quaternion.Euler(0, 0, angle) * dir;
    }

    private void ShootShotgun(Vector2 baseDir)
    {
        for (int i = 0; i < 5; i++)
        {
            Vector2 dir = ApplySpread(baseDir, 45f);
            ShootProjectile(stats.weaponRanged, dir, 0.2f); // 20% daño cada perdigón
        }
    }

    private void ShootProjectile(SOWeapon weapon, Vector2 direction, float dmgMultiplier)
    {
        Vector2 firePoint = GetDynamicFirePoint(direction);

        GameObject projObj = Instantiate(weapon.weaponPrefab, firePoint, Quaternion.identity);

        Projectile p = projObj.GetComponent<Projectile>();
        p.direction = direction;
        p.damageMultiplier = dmgMultiplier;
        p.owner = stats;
    }
}
