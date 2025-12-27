using UnityEngine;

/**Clase abstracta para la pasiva del personaje*/
public abstract class SOClassPassive : ScriptableObject
{
    public abstract void Activate(PlayerStats stats);
    public abstract void Deactivate(PlayerStats stats);
}