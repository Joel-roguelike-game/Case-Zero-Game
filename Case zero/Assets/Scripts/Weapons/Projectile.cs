using UnityEngine;

/*
 Control del proyectil básico:
 - Se mueve en línea recta
 - Destruye al impactar
*/
public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public Vector2 direction;
    public float lifeTime = 3f;

    public float damageMultiplier = 1f;
    public PlayerStats owner;

    private float timer;

    private void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= lifeTime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Destruye proyectiles al impactar enemigos o salir de límites
        if(other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        // Destruye proyectiles al impactar enemigos o salir de límites
        if(other.CompareTag("MapBoundary"))
        {
            Destroy(gameObject);
        }
    }

}