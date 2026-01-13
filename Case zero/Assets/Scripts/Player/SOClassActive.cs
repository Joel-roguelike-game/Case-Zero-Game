using UnityEngine;
/**Clase abstracta para la activa del personaje*/

public abstract class SOClassActive : ScriptableObject
{
    public float focusCost = 100f; // coste base
    public abstract void Activar(PlayerStats stats);
}

