using UnityEngine;

[CreateAssetMenu(
    fileName = "DamageTextConfig_Dot",
    menuName = "Combat/Damage Text Config (DOT)"
)]
public class SODamageTextConfig_Dot : ScriptableObject
{
    [Header("Prefab")]
    public GameObject damageTextPrefab;

    [Header("DOT Hit")]
    public Color dotColor = new Color(0.7f, 0f, 1f);
    public float dotScale = 0.9f;

    [Header("DOT Critical")]
    public Color dotCritColor = new Color(1f, 0.1f, 0.4f);
    public float dotCritScale = 1.1f;

    [Header("Motion")]
    public float floatSpeed = 1.2f;
    public float lifeTime = 1.2f;

    [Header("Offset")]
    public Vector3 worldOffset = new Vector3(0f, -0.35f, 0f);
}