using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private float lifeTime = 1f;

    public void Initialize(int value, bool isCrit, SODamageTextConfig config)
    {
        if (text == null)
        {
            Debug.LogError("DamageText: TextMeshPro no asignado");
            return;
        }

        if (config == null)
        {
            Debug.LogError("DamageText: Config es NULL");
            return;
        }

        text.text = value.ToString();
        text.color = isCrit ? config.critColor : config.normalColor;
        text.fontSize = isCrit ? config.critScale : config.normalScale;

        Destroy(gameObject, lifeTime);
    }
}