public class Tower : Building
{
    private float currentRange;
    private float currentDamage;

    protected override void UpdateStats()
    {
        base.UpdateStats();
        TowerData towerData = (TowerData)data;
        currentRange = towerData.attackRange * towerData.rangeMultipliers[currentLevel - 1];
        currentDamage = towerData.damage * towerData.damageMultipliers[currentLevel - 1];
    }
}