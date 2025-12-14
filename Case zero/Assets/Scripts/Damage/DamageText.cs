using UnityEngine;
using TMPro;

/*
 Controla el comportamiento visual del texto de daño.
 */
public class DamageText : MonoBehaviour
{
    private TextMeshPro text;
    private float lifeTime;
    private float floatSpeed;

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
    }

    public void Initialize(int value, bool isCrit, SODamageTextConfig config)
    {
        text.text = value.ToString();

        if (isCrit)
        {
            text.color = config.critColor;
            transform.localScale *= config.critScale;
        }
        else
        {
            text.color = config.normalColor;
            transform.localScale *= config.normalScale;
        }

        lifeTime = config.lifeTime;
        floatSpeed = config.floatSpeed;
    }

    private void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
            Destroy(gameObject);
    }
}