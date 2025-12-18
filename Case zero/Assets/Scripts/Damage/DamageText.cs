using UnityEngine;
using TMPro;
/*gestiona el texto de daño con su configuracion. */
public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    private float timer;
    private float lifetime;
    private int accumulatedDamage;

    public void Initialize(
        int damage,
        bool isCrit,
        SODamageTextConfig config
    )
    {
        if (text == null)
        {
            Debug.LogError("DamageText: TextMeshPro no asignado");
            return;
        }

        accumulatedDamage = damage;
        lifetime = config.lifeTime;
        timer = 0f;

        ApplyVisual(isCrit, config);
        UpdateText();
    }
    
    /*Acumula daño en el texto */
    public void AddDamage(
        int damage,
        bool isCrit,
        SODamageTextConfig config
    )
    {
        accumulatedDamage += damage;
        timer = 0f; // reset lifetime

        ApplyVisual(isCrit, config);
        UpdateText();
    }

    /*Muestra texto blanco con daño normal, meustra texto amarillo con daño critico. */
    private void ApplyVisual(bool isCrit, SODamageTextConfig config)
    {
        text.color = isCrit ? config.critColor : config.normalColor;
        text.fontSize = isCrit ? config.critScale : config.normalScale;
    }
    /*Daño acumulativo. */
    private void UpdateText()
    {
        text.text = accumulatedDamage.ToString();
    }
    /*controla el tiempo de vida del texto. */
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifetime)
            Destroy(gameObject);
    }
    /*si es daño acumulado, esto ajusta otra vez la posicion del texto */
    public void SetWorldPosition(Vector3 pos)
    {
        transform.position = pos;
    }

}