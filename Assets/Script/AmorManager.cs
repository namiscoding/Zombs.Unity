using TMPro;
using UnityEngine;

public class AmorManager : MonoBehaviour
{
    [SerializeField] private int currentLevel = 0;
    private float[] maxHealthBonuses = { 0f, 20f, 40f, 60f, 80f, 100f };
    private float[] damageReductionPercentages = { 0f, 0.05f, 0.1f, 0.15f, 0.2f, 0.25f };
    private int[] upgradeCosts = { 50, 100, 150, 200, 250 };
    private float currentArmor;

    public TextMeshProUGUI armorPrice;
    public TextMeshProUGUI currentHPText; // HP hiện tại
    public TextMeshProUGUI nextHPText; // HP sau khi nâng cấp
    public TextMeshProUGUI currentDamageReductionText; // Damage giảm hiện tại
    public TextMeshProUGUI nextDamageReductionText; // Damage giảm sau khi nâng cấp

    void Start()
    {
        if (maxHealthBonuses.Length != damageReductionPercentages.Length || upgradeCosts.Length != maxHealthBonuses.Length - 1)
        {
            Debug.LogError("AmorManager configuration mismatch!");
        }
        if (armorPrice == null || currentHPText == null || nextHPText == null ||
            currentDamageReductionText == null || nextDamageReductionText == null)
        {
            Debug.LogError("One or more TextMeshProUGUI fields not assigned in Inspector!");
        }
        ResetArmor();
        UpdateArmorPriceUI();
    }

    public void ResetArmor()
    {
        currentArmor = GetMaxArmor();
        SyncArmorWithUI();
    }

    public float GetMaxArmor()
    {
        return currentLevel > 0 ? 10f * currentLevel : 0f;
    }

    public float GetCurrentArmor()
    {
        return currentArmor;
    }

    public float GetMaxHealthBonus()
    {
        return currentLevel < maxHealthBonuses.Length ? maxHealthBonuses[currentLevel] : maxHealthBonuses[maxHealthBonuses.Length - 1];
    }

    public float GetDamageReduction()
    {
        return currentLevel < damageReductionPercentages.Length ? damageReductionPercentages[currentLevel] : damageReductionPercentages[damageReductionPercentages.Length - 1];
    }

    public bool UpgradeArmor(PlayerManager playerManager)
    {
        if (currentLevel >= upgradeCosts.Length)
        {
            UpdateArmorPriceUI();
            return false;
        }

        int nextLevel = currentLevel + 1;
        if (playerManager.gold >= upgradeCosts[currentLevel])
        {
            playerManager.gold -= upgradeCosts[currentLevel];
            currentLevel = nextLevel;
            ResetArmor();
            playerManager.SyncWithAmorManager();
            UpdateArmorPriceUI();
            return true;
        }
        return false;
    }

    public void TakeDamage(float damage)
    {
        currentArmor = Mathf.Max(0, currentArmor - damage);
        SyncArmorWithUI();
    }

    public void SetCurrentArmor(float value)
    {
        currentArmor = Mathf.Clamp(value, 0, GetMaxArmor());
        SyncArmorWithUI();
    }

    private void SyncArmorWithUI()
    {
        ArmorPlayer armorPlayer = FindFirstObjectByType<ArmorPlayer>();
        if (armorPlayer != null)
        {
            armorPlayer.UpdatePlayerArmor(currentArmor, GetMaxArmor());
        }
    }

    private void UpdateArmorPriceUI()
    {
        if (armorPrice != null)
        {
            if (currentLevel >= upgradeCosts.Length)
            {
                armorPrice.text = "Max Level";
            }
            else
            {
                armorPrice.text = upgradeCosts[currentLevel].ToString();
            }
        }

        // Cập nhật HP hiện tại
        if (currentHPText != null)
        {
            currentHPText.text = GetMaxHealthBonus().ToString();
        }

        // Cập nhật HP sau khi nâng cấp
        if (nextHPText != null)
        {
            if (currentLevel < maxHealthBonuses.Length - 1)
            {
                nextHPText.text = maxHealthBonuses[currentLevel + 1].ToString();
            }
            else
            {
                nextHPText.text = "Max";
            }
        }

        // Cập nhật Damage giảm hiện tại
        if (currentDamageReductionText != null)
        {
            currentDamageReductionText.text = (GetDamageReduction() * 100).ToString("F0") + "%";
        }

        // Cập nhật Damage giảm sau khi nâng cấp
        if (nextDamageReductionText != null)
        {
            if (currentLevel < damageReductionPercentages.Length - 1)
            {
                nextDamageReductionText.text = (damageReductionPercentages[currentLevel + 1] * 100).ToString("F0") + "%";
            }
            else
            {
                nextDamageReductionText.text = "Max";
            }
        }
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }
}