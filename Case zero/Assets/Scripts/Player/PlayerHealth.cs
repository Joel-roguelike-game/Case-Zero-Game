using UnityEngine;
using System.Collections;

/*
 Controla la vida del jugador y la invulnerabilidad.
*/
public class PlayerHealth : MonoBehaviour
{
    public float currentHealth = 100f;
    public float invulDuration = 1f;

    private bool invulnerable;

    public void TakeDamage(float dmg)
    {
        if (invulnerable)
            return;

        currentHealth -= dmg;
        StartCoroutine(Invulnerability());
    }

    private IEnumerator Invulnerability()
    {
        invulnerable = true;
        yield return new WaitForSeconds(invulDuration);
        invulnerable = false;
    }
}