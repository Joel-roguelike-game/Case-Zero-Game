using UnityEngine;

[CreateAssetMenu(
    fileName = "DamageTextConfig",
    menuName = "Combat/Damage Text Config"
)]
public class SODamageTextConfig : ScriptableObject
{
    [Header("Prefab")]
    public GameObject damageTextPrefab;

    [Header("Normal Hit")]
    public Color normalColor = Color.white;
    public float normalScale = 1f;

    [Header("Critical Hit")]
    public Color critColor = Color.yellow;
    public float critScale = 1.3f;

    [Header("Motion")]
    public float floatSpeed = 1.5f;
    public float lifeTime = 1f;
}