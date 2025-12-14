using UnityEngine;

/*
 Se encarga de mostrar el texto de daño en pantalla.
 */
public class DamageTextSpawner : MonoBehaviour
{
    public static DamageTextSpawner Instance;

    [SerializeField]
    private SODamageTextConfig config;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Spawn(Vector3 worldPos, float damage, bool isCrit)
    {
        if (config == null || config.damageTextPrefab == null)
        {
            Debug.LogError("DamageTextConfig o prefab no asignado");
            return;
        }

        GameObject obj = Instantiate(
            config.damageTextPrefab,
            worldPos,
            Quaternion.identity
        );

        DamageText text = obj.GetComponent<DamageText>();
        if (text == null)
        {
            Debug.LogError("El prefab no tiene DamageText");
            return;
        }

        text.Initialize(
            Mathf.FloorToInt(damage),
            isCrit,
            config
        );
    }
}