using UnityEngine;

/*
 Representa un ataque cuerpo a cuerpo:
 - Se orienta hacia el ratón
 - Posee un ángulo de 90 grados
 - Escala según CaCRange del jugador
*/
public class MeleeSlash : MonoBehaviour
{
    public PlayerStats owner;
    public Vector2 direction;

    public float duration = 0.15f;
    private float timer;

    private void Start()
    {
        // Rotar hacia el ratón
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 45f); // slash centrado

        // Escalar según alcance del jugador
        float range = owner.caCRange.Current;
        transform.localScale = new Vector3(range, range, 1f);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // TODO: aplicar daño al enemigo
    }
}