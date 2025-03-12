using UnityEngine;

[CreateAssetMenu(fileName = "NewTower", menuName = "Game/Tower Data")]
public class TowerData : ScriptableObject
{
    [Header("General")]
    public string towerName;
    public Sprite sprite;
    public int maxQuantity;

    [Header("Base Stats (Level 1)")]
    public float attackRange;
    public int maxHealth;
    public float attackSpeed;
    public int damage;

    [System.Serializable]
    public struct ResourceCost
    {
        public int wood;
        public int rock;
        public int gold;
    }

    [Header("Resource Costs")]
    public ResourceCost baseCost;         // Cost to place at Level 1
    public ResourceCost[] levelUpCosts;   // Costs for levels 2-5 (array size 4)

    [Header("Level Multipliers")]
    public float[] healthMultipliers;     // Array size 5 (Levels 1-5)
    public float[] damageMultipliers;     // Array size 5
    public float[] rangeMultipliers;      // Array size 5
}