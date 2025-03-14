using UnityEngine;

[CreateAssetMenu(fileName = "NewGoldMiner", menuName = "Buildings/Gold Miner Data")]
public class GoldMinerData : BuildingData
{
    public float goldRate; // Gold per second
    public float[] goldRateMultipliers; // Size 5
}