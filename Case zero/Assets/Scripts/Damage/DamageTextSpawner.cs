using UnityEngine;

public class DamageTextSpawner : MonoBehaviour
{
    public static DamageTextSpawner Instance;

    public DamageText prefab;
    public SODamageTextConfig config;

    private void Awake()
    {
        Instance = this;
    }

    public DamageText Spawn(
        Vector3 worldPos,
        int damage,
        bool isCrit
    )
    {
        DamageText dt = Instantiate(prefab, worldPos, Quaternion.identity);
        dt.Initialize(damage, isCrit, config);
        return dt;
    }
}