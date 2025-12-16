using UnityEngine;
using TMPro;

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

    private void ApplyVisual(bool isCrit, SODamageTextConfig config)
    {
        text.color = isCrit ? config.critColor : config.normalColor;
        text.fontSize = isCrit ? config.critScale : config.normalScale;
    }

    private void UpdateText()
    {
        text.text = accumulatedDamage.ToString();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifetime)
            Destroy(gameObject);
    }
    
    public void SetWorldPosition(Vector3 pos)
    {
        transform.position = pos;
    }

}