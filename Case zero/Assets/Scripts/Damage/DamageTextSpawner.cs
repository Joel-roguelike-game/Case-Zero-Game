using UnityEngine;
/*hace aparecer el texto de daño*/
public class DamageTextSpawner : MonoBehaviour
{
    public static DamageTextSpawner Instance;

    public DamageText prefab;
    public SODamageTextConfig config;

    private void Awake()
    {
        Instance = this;
    }
    /*hace aparecer el texto de daño en el mundo*/
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