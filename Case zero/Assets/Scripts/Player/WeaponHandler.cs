using UnityEngine;
using UnityEngine.InputSystem;

/*
 Gestiona ataques melee y ranged del jugador.
 Calcula firePoint dinámico según el borde del collider y la dirección del ratón.
 Maneja ataque continuo según velocidad de ataque real.
 Ignora colisiones entre jugador y melee slash.
*/
public class WeaponHandler : MonoBehaviour
{
    private PlayerStats stats;
    private Collider2D playerCollider;
    private Camera cam;
    private PlayerInputController inputController;

    private float meleeCooldownTimer = 0f;
    private float rangedCooldownTimer = 0f;

    private bool meleeHeld;
    private bool rangedHeld;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        playerCollider = GetComponent<Collider2D>();
        cam = Camera.main;
        inputController = GetComponent<PlayerInputController>();
        if(playerCollider == null) Debug.LogError("WeaponHandler: No Collider2D found on this GameObject!");
        if(stats == null) Debug.LogError("WeaponHandler: No PlayerStats found on this GameObject!");
        
    }
    
    private void Start()
    {
        if(inputController == null) inputController = GetComponent<PlayerInputController>();
        if(inputController.inputActions == null)
        {
            Debug.LogError("WeaponHandler: inputActions not initialized!");
            return;
        }

        // Suscribir eventos, aqui puesto que el orden de awakes no esta garantizado
        inputController.inputActions.Gameplay.MeleeAttack.performed += ctx => meleeHeld = true;
        inputController.inputActions.Gameplay.MeleeAttack.canceled += ctx => meleeHeld = false;

        inputController.inputActions.Gameplay.RangedAttack.performed += ctx => rangedHeld = true;
        inputController.inputActions.Gameplay.RangedAttack.canceled += ctx => rangedHeld = false;
    }


    private void Update()
    {
        // Ataque cuerpo a cuerpo
        if (meleeHeld && meleeCooldownTimer <= 0f)
        {
            UseMelee();
            meleeCooldownTimer = 1f / (stats.weaponMelee.attackSpeed * stats.atkSpeedCaC.Current);
        }

        // Ataque a distancia
        if (rangedHeld && rangedCooldownTimer <= 0f)
        {
            UseRanged();
            rangedCooldownTimer = 1f / (stats.weaponRanged.attackSpeed * stats.atkSpeedDist.Current);
        }

        meleeCooldownTimer -= Time.deltaTime;
        rangedCooldownTimer -= Time.deltaTime;
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

    public void UseMelee()
    {
        if (stats.weaponMelee == null) return;

        Vector2 dir = GetMouseDirection();
        float radius = playerCollider.bounds.extents.magnitude * stats.caCRange.Current;
        Vector2 slashPos = (Vector2)playerCollider.bounds.center + dir * radius;

        GameObject slashObj = Instantiate(stats.weaponMelee.weaponPrefab, slashPos, Quaternion.identity);
        MeleeSlash slash = slashObj.GetComponent<MeleeSlash>();
        slash.owner = stats;
        slash.direction = dir;

        // Ignorar colisión con jugador
        Collider2D slashCol = slashObj.GetComponent<Collider2D>();
        if (slashCol != null)
            Physics2D.IgnoreCollision(playerCollider, slashCol);
    }

    public void UseRanged()
    {
        if (stats.weaponRanged == null) return;
        if (stats.actualAmmo.Current <= 0) return;

        Vector2 baseDir = GetMouseDirection();
        float spread = GetSpread(stats.weaponRanged);

        if (stats.weaponRanged.weaponName == "Escopeta")
            ShootShotgun(baseDir);
        else
        {
            Vector2 dirFinal = ApplySpread(baseDir, spread);
            ShootProjectile(stats.weaponRanged, dirFinal, 1f);
        }

        stats.actualAmmo.Current--;
    }

    private float GetSpread(SOWeapon weapon)
    {
        if (weapon.weaponName == "Ballesta" || weapon.weaponName == "Rifle") return 0f;
        return weapon.weaponName == "Escopeta" ? 45f : 15f;
    }

    private Vector2 ApplySpread(Vector2 dir, float spread)
    {
        float angle = Random.Range(-spread / 2f, spread / 2f);
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
        GameObject projObj = Instantiate(weapon.weaponPrefab, firePoint, Quaternion.identity);
        Projectile p = projObj.GetComponent<Projectile>();
        p.direction = direction;
        p.damageMultiplier = dmgMultiplier;
        p.owner = stats;
    }
}
