using UnityEngine;
using UnityEngine.InputSystem;

/*
 Gestiona el uso de armas del jugador.
 Controla ataques melee y ranged, cadencia real,
 prioridad entre ataques y cálculo de dirección.
*/
public class WeaponHandler : MonoBehaviour
{
    private PlayerStats stats;
    private Collider2D playerCollider;
    private Camera cam;
    private PlayerInputController inputController;

    private float nextAttackTime = 0f;

    public bool isAttacking;

    /*
     * Inicializa referencias necesarias.
     */
    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        playerCollider = GetComponent<Collider2D>();
        inputController = GetComponent<PlayerInputController>();
        cam = Camera.main;
    }

    /*
     * Gestiona el input continuo de ataque.
     * Controla la cadencia real usando Time.time.
     */
    private void Update()
    {
        isAttacking = false;
        
        if (Time.time < nextAttackTime)
            return;

        bool meleeHeld  = inputController.inputActions.Gameplay.MeleeAttack.ReadValue<float>() > 0.1f;
        bool rangedHeld = inputController.inputActions.Gameplay.RangedAttack.ReadValue<float>() > 0.1f;

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

    /*
     * Calcula el cooldown real del ataque melee.
     */
    private float GetMeleeCooldown()
    {
        return 1f / (stats.weaponMelee.attackSpeed * stats.atkSpeedCaC.Current);
    }

    /*
     * Calcula el cooldown real del ataque a distancia.
     */
    private float GetRangedCooldown()
    {
        return 1f / (stats.weaponRanged.attackSpeed * stats.atkSpeedDist.Current);
    }

    /*
     * Obtiene la dirección desde el jugador hacia el ratón.
     */
    private Vector2 GetMouseDirection()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);
        return (mouseWorldPos - (Vector2)transform.position).normalized;
    }

    /*
     * Calcula dinámicamente el punto de salida del proyectil.
     */
    private Vector2 GetDynamicFirePoint(Vector2 direction)
    {
        float radius = playerCollider.bounds.extents.magnitude;
        return (Vector2)playerCollider.bounds.center + direction * radius;
    }

    /*
     * Ejecuta un ataque cuerpo a cuerpo.
     */
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

        slash.damage = ((stats.weaponMelee.damagePercent / 100) * stats.caCDmg.Current) + stats.weaponMelee.flatDamage;
        slash.stabilityBreak = stats.weaponMelee.stabilityBreak;
        slash.stabilityMultiplier = stats.stabilityMultiplier.Current;

        Collider2D slashCol = slashObj.GetComponent<Collider2D>();
        if (slashCol != null)
            Physics2D.IgnoreCollision(playerCollider, slashCol);
    }

    /*
     * Ejecuta un ataque a distancia.
     * Gestiona munición y armas especiales como la escopeta.
     */
    public void UseRanged()
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
            Vector2 dir = ApplySpread(baseDir, GetSpread(stats.weaponRanged));
            ShootProjectile(stats.weaponRanged, dir, 1f);
        }

        stats.currentAmmo--;
    }

    /*
     * Devuelve el spread según el arma.
     */
    private float GetSpread(SOWeapon weapon)
    {
        if (weapon.weaponName == "Ballesta" || weapon.weaponName == "Rifle")
            return 0f;

        return weapon.weaponName == "Escopeta" ? 25f : 10f;
    }

    /*
     * Aplica variación angular a la dirección del disparo.
     */
    private Vector2 ApplySpread(Vector2 dir, float spread)
    {
        float angle = Random.Range(-spread * 0.5f, spread * 0.5f);
        return Quaternion.Euler(0, 0, angle) * dir;
    }

    /*
     * Dispara múltiples proyectiles tipo escopeta.
     */
    private void ShootShotgun(Vector2 baseDir)
    {
        for (int i = 0; i < 5; i++)
        {
            Vector2 dir = ApplySpread(baseDir, 25f);
            ShootProjectile(stats.weaponRanged, dir, 1f);
        }
    }

    /*
     * Instancia y configura un proyectil.
     */
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
