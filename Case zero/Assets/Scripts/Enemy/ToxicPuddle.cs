using UnityEngine;

public class ToxicPuddle : MonoBehaviour
{
    public float lifeTime = 5f;
    public float damagePerSecond = 5f;
    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifeTime)
            Destroy(gameObject);
    }

    public float tickDamage = 5f;
    public float tickInterval = 0.5f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
            other.GetComponent<PlayerHealth>()
                ?.TakeDamage(tickDamage, null);
    }
}
    