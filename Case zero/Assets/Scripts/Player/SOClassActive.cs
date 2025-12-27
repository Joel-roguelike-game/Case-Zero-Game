using UnityEngine;
/**Clase abstracta para la activa del personaje*/

public abstract class SOClassActive : ScriptableObject
{
    public float cooldown = 10f; // cooldown base
    public abstract void Activar(PlayerStats stats);
}

