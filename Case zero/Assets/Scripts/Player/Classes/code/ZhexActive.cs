using UnityEngine;

[CreateAssetMenu(menuName = "Classes/Actives/ZhexActive")]
public class ZhexActive : SOClassActive
{
    public GameObject ritualPrefab;

    public override void Activar(PlayerStats stats)
    {
        if (!stats.ConsumeFocus(60f))
            return;

        GameObject zone = Instantiate(
            ritualPrefab,
            stats.transform.position,
            Quaternion.identity
        );

        zone.GetComponent<ZhexRitualZone>().Init(stats);
    }
}