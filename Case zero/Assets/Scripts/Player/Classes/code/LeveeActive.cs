using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

/*
 * LeveeActive
 * 
 * Activa: HYPERCONCENTRACION
 * - Si hay un enemigo bajo el cursor, recibe +30% daño durante 10s
 * - Matar al objetivo antes de que termine reduce cooldown a la mitad y aumenta prob. crítica en 20% por 10s
 * - Cooldown base: 40s
 */
[CreateAssetMenu(fileName = "LeveeActive", menuName = "Classes/Actives/LeveeActive")]
public class LeveeActive : SOClassActive
{
    public float duration = 10f;
    public float damageMultiplier = 1.3f;
    public float critBonus = 20f; // +20% prob crit
    public float killBuffDuration = 10f;

    private EnemyCombat targetEnemy;

    public override void Activar(PlayerStats stats)
    {
        // Buscamos enemigo bajo cursor
        targetEnemy = FindEnemyUnderCursor();
        if (targetEnemy == null)
        {
            //Debug.Log("No hay enemigo bajo cursor. Activa no usada.");
            return; // no se activa ni se consume cooldown
        }

        // Solo se registra el uso si hay enemigo
        stats.lastActiveTime = Time.time; // <--- mueve aquí
        Debug.Log("Activa HYPERCONCENTRACION activada");
        stats.StartCoroutine(ApplyActive(stats));
    }



    /*
     * Rutina que aplica el efecto sobre el enemigo seleccionado
     */
    private IEnumerator ApplyActive(PlayerStats stats)
    {
        // targetEnemy
        targetEnemy = FindEnemyUnderCursor();
        if (targetEnemy == null) yield break;
        else Debug.Log("enemigo correctamente hyperconcentrado.");

        targetEnemy.damageMultiplier *= damageMultiplier;

        // Guardamos el tiempo de inicio
        float startTime = Time.time;
        Debug.Log("EN COOLDOWN");

        // Bucle durante la duración
        while (Time.time < startTime + duration)
        {
            if (targetEnemy.health.isDead)
            { 
                Debug.Log("Objetivo muerto durante HYPERCONCENTRACION, aplicando bonus crit");
                stats.critChance.Current += critBonus;
                Debug.Log("Cooldown sin reducir, next usable en: " + (stats.lastActiveTime + stats.classActive.cooldown));

                float elapsed = Time.time - stats.lastActiveTime;
                float remainingCooldown = stats.classActive.cooldown - elapsed;
                stats.lastActiveTime -= remainingCooldown * 0.5f; //ajusta el cooldown restante
                Debug.Log("Cooldown reducido, next usable en: " + (stats.lastActiveTime + stats.classActive.cooldown));
                yield return new WaitForSeconds(killBuffDuration);
                Debug.Log("buff de crit terminado");
                stats.critChance.Current -= critBonus;
                break;
            }
            yield return null;
        }
        Debug.Log("HABILIDAD TERMINO!");
        if (!targetEnemy.health.isDead)
            targetEnemy.damageMultiplier /= damageMultiplier;
    }


    /*
     * Detecta enemigo bajo cursor
     */
    private EnemyCombat FindEnemyUnderCursor()
    {
        // Obtenemos la posición del mouse en pantalla
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();

        // Convertimos a world position usando z = 10 (distancia desde cámara)
        // Porque la cámara está en z = -10 y el plano de juego en z = 0
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 10f));

        // Usamos OverlapCircle para tolerancia, 0.1f de radio
        Collider2D hit = Physics2D.OverlapCircle(mouseWorldPos, 0.1f);
        if (hit != null && hit.CompareTag("Enemy"))
        {
            return hit.GetComponent<EnemyCombat>();
        }

        //Debug.Log("No hay enemigo bajo el cursor: " + mouseWorldPos);
        return null;
    }

}
