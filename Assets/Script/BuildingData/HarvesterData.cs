using UnityEngine;

[CreateAssetMenu(fileName = "NewHarvester", menuName = "Buildings/Harvester Data")]
public class HarvesterData : BuildingData
{
    public float harvestRate; // Resources per harvest
    public int storageLimit;  // Max wood/rock stored
    public float[] harvestRateMultipliers; // Size 5
}