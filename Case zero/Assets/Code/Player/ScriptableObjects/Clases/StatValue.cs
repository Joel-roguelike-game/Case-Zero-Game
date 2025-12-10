[System.Serializable]

/*estructura para separar en la misma variable el valor base del valor actual*/
public struct StatValue
{
    public float Base;
    public float Current;

    public StatValue(float value)
    {
        Base = value;
        Current = value;
    }

    public void Reset() => Current = Base;
}