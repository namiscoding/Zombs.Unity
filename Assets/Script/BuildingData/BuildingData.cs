using UnityEngine;

public abstract class BuildingData : ScriptableObject
{
    public string buildingName;
    public Sprite sprite;
    public int maxQuantity;
    public int maxHealth;
    public ResourceCost baseCost;
    public ResourceCost[] levelUpCosts; // Size 4 for levels 2-5
    public float[] healthMultipliers;   // Size 5 for levels 1-5

    [System.Serializable]
    public struct ResourceCost
    {
        public int wood;
        public int rock;
        public int gold;
    }
}