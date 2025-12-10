using UnityEngine;

/**Clase abstracta para la pasiva del personaje*/
public abstract class PlayerClassPassive : ScriptableObject
{
    public abstract void Activar(PlayerStats stats);
}