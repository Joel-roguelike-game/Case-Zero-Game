using UnityEngine;

/**Clase abstracta para la pasiva del personaje*/
public abstract class SOClassPassive : ScriptableObject
{
    public abstract void Activar(PlayerStats stats);
}