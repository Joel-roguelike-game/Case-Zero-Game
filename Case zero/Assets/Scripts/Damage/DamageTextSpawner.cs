using UnityEngine;

public class DamageTextSpawner : MonoBehaviour
{
    public static DamageTextSpawner Instance;

    [SerializeField] private DamageText damageTextPrefab;
    [SerializeField] private SODamageTextConfig config;

    private void Awake()
    {
        Instance = this;
    }

    public void Spawn(Vector3 worldPos, float damage, bool isCrit)
    {
        if (damageTextPrefab == null)
        {
            Debug.LogError("DamageTextSpawner: Prefab NULL");
            return;
        }

        DamageText dt = Instantiate(
            damageTextPrefab,
            worldPos,
            Quaternion.identity
        );

        dt.Initialize(Mathf.FloorToInt(damage), isCrit, config);
    }
}