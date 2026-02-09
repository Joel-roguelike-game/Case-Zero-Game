using UnityEngine;

public class DotDamageTextSpawner : MonoBehaviour
{
    public static DotDamageTextSpawner Instance;

    public DamageText prefab;
    public SODamageTextConfig_Dot config;

    private void Awake()
    {
        Instance = this;
    }

    public void Spawn(
        Vector3 worldPos,
        int damage,
        bool isCrit
    )
    {
        Vector3 pos = worldPos + config.worldOffset;

        DamageText dt = Instantiate(prefab, pos, Quaternion.identity);
        dt.Initialize(
            damage,
            isCrit,
            ConvertConfig(isCrit)
        );
    }

    private SODamageTextConfig ConvertConfig(bool isCrit)
    {
        // puente rápido sin reescribir DamageText
        SODamageTextConfig cfg = ScriptableObject.CreateInstance<SODamageTextConfig>();
        cfg.normalColor = config.dotColor;
        cfg.critColor = config.dotCritColor;
        cfg.normalScale = config.dotScale;
        cfg.critScale = config.dotCritScale;
        cfg.lifeTime = config.lifeTime;
        return cfg;
    }
}