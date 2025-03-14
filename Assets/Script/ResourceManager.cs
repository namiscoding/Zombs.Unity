using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    public int wood { get; private set; } = 200; // Starting values
    public int rock { get; private set; } = 200;
    public int gold { get; private set; } = 200;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool CanAfford(TowerData.ResourceCost cost)
    {
        return wood >= cost.wood && rock >= cost.rock && gold >= cost.gold;
    }

    public void SpendResources(TowerData.ResourceCost cost)
    {
        wood -= cost.wood;
        rock -= cost.rock;
        gold -= cost.gold;
        Debug.Log($"Resources left: Wood={wood}, Rock={rock}, Gold={gold}");
    }

    // For testing or resource gathering
    public void AddResources(int w, int r, int g)
    {
        wood += w;
        rock += r;
        gold += g;
    }
}