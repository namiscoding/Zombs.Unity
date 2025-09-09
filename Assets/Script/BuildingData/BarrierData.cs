using UnityEngine;

[CreateAssetMenu(fileName = "NewBarrier", menuName = "Buildings/Barrier Data")]
public class BarrierData : BuildingData
{
    public bool isDoor; // True for Door, false for Wall
}