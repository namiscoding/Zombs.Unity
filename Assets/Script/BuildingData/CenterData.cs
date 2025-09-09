using UnityEngine;

[CreateAssetMenu(fileName = "NewCenter", menuName = "Buildings/Center Data")]
public class CenterData : BuildingData
{
    public float baseRange; // Range of base influence
    public float[] rangeMultipliers; // Size 5 for levels 1-5
}