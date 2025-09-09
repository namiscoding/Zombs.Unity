using UnityEngine;

public class Harvester : Building
{
    private float currentHarvestRate;
    private int storedWood = 0;
    private int storedRock = 0;
    private float harvestTimer;

    protected override void UpdateStats()
    {
        base.UpdateStats();
        HarvesterData harvesterData = (HarvesterData)data;
        currentHarvestRate = harvesterData.harvestRate * harvesterData.harvestRateMultipliers[currentLevel - 1];
    }

    void Update()
    {
        harvestTimer += Time.deltaTime;
        if (harvestTimer >= 1f && CanHarvest())
        {
            Harvest();
            harvestTimer = 0f;
        }
    }

    private bool CanHarvest()
    {
        HarvesterData harvesterData = (HarvesterData)data;
        return storedWood + storedRock < harvesterData.storageLimit;
    }

    private void Harvest()
    {
        // Placeholder: Check for nearby rock/tree via overlap
        if (Physics2D.OverlapCircle(transform.position, 1f, LayerMask.GetMask("Resources")))
        {
            HarvesterData harvesterData = (HarvesterData)data;
            storedWood += (int)currentHarvestRate; // Simplified: assume wood for now
            storedWood = Mathf.Min(storedWood, harvesterData.storageLimit);
        }
    }

    void OnMouseDown() // Player clicks to collect
    {
        ResourceManager.Instance.AddResources(storedWood, storedRock, 0);
        storedWood = 0;
        storedRock = 0;
    }
}