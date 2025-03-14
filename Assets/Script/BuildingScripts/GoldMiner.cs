public class GoldMiner : Building
{
    private float currentGoldRate;

    protected override void Start()
    {
        base.Start();
        InvokeRepeating(nameof(GenerateGold), 1f, 1f); // Generate gold every second
    }

    protected override void UpdateStats()
    {
        base.UpdateStats();
        GoldMinerData minerData = (GoldMinerData)data;
        currentGoldRate = minerData.goldRate * minerData.goldRateMultipliers[currentLevel - 1];
    }

    private void GenerateGold()
    {
        ResourceManager.Instance.AddResources(0, 0, (int)currentGoldRate);
    }
}