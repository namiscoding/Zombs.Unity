using UnityEngine;

[CreateAssetMenu(fileName = "NewTower", menuName = "Buildings/Tower Data")]
public class TowerData : BuildingData
{
    public float attackRange;
    public float attackSpeed;
    public int damage;
    public float[] damageMultipliers; // Size 5
    public float[] rangeMultipliers;  // Size 5
}