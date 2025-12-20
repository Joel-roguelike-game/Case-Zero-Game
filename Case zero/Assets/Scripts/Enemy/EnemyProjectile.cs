using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 8f;
    public float lifeTime = 4f;

    private Vector2 direction;
    private float damage;
    private float timer;

    public void Init(Vector2 dir, float dmg)
    {
        direction = dir.normalized;
        damage = dmg;
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= lifeTime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()
                ?.TakeDamage(damage, null);
            Destroy(gameObject);
        }

        if (other.CompareTag("MapBoundary"))
            Destroy(gameObject);
    }
}