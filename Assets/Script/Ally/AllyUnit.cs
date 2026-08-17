using UnityEngine;

public class AllyUnit : MonoBehaviour
{
    private AllyData allyData;

    public AllyData Data
    {
        get { return allyData; }
    }

    public void Initialize(AllyData data)
    {
        allyData = data;
    }
}