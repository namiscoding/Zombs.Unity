using UnityEngine;
using UnityEngine.UI;

public class ArmorPlayer : MonoBehaviour
{
    [SerializeField] private Slider armor;
    private AmorManager amorManager;

    void Start()
    {
        if (armor == null) Debug.LogError("Armor Slider not assigned in Inspector!");
        armor.interactable = false;
        armor.minValue = 0;
        armor.maxValue = 1;
        armor.value = armor.maxValue; // Đặt giá trị mặc định bằng maxValue
        amorManager = FindFirstObjectByType<AmorManager>();
        if (amorManager == null) Debug.LogError("AmorManager not found!");
        UpdatePlayerArmor(armor.maxValue, armor.maxValue); // Cập nhật thanh armor
    }

    public void UpdatePlayerArmor(float currentValue, float maxValue)
    {
        if (armor != null)
        {
            armor.value = maxValue > 0 ? currentValue / maxValue : 0;
        }
    }
}