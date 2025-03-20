using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    public int wood { get; private set; } = 200; 
    public int stone { get; private set; } = 200;
    public int gold { get; private set; } = 200;
    [SerializeField] private TextMeshProUGUI txtWood;     
    [SerializeField] private TextMeshProUGUI txtStone;    
    [SerializeField] private TextMeshProUGUI txtGold;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void UpdateResourceUI()
    {
        txtWood.text = "" + wood;
        txtStone.text = "" + stone;
        //txtWood.text = "Wood: " + wood;
        //txtStone.text = "Stone: " + stone;
    }
    public bool CanAfford(TowerData.ResourceCost cost)
    {
        return wood >= cost.wood && stone >= cost.stone && gold >= cost.gold;
    }

    public void SpendResources(TowerData.ResourceCost cost)
    {
        wood -= cost.wood;
        stone -= cost.stone;
        gold -= cost.gold;
        Debug.Log($"Resources left: Wood={wood}, Stone={stone}, Gold={gold}");
    }

    // For testing or resource gathering
    public void AddResources(int w, int s, int g)
    {
        wood += w;
        stone += s;
        gold += g;
    }
    public void AddStone(int quality)
    {
        stone += quality;      // Cộng số lượng đá
        UpdateResourceUI();    // Cập nhật lại UI
    }

    // Thêm gỗ
    public void AddWood(int quality)
    {
        wood += quality;       // Cộng số lượng gỗ
        UpdateResourceUI();    // Cập nhật lại UI
    }
}