using TMPro;
using UnityEngine;

public class WP_BowManager : MonoBehaviour
{
    [SerializeField] private int currentLevel = 0; // 0 = không có cung, 1-5 = cấp độ cung
    private float[] damage = { 0f, 15f, 25f, 35f, 45f, 55f, 65f }; // damage[0] = 0 (không có cung)
    private int[] upgradeCosts = { 200, 500, 1000, 1500, 2000, 2500 }; // Chi phí nâng cấp
    private float currentBowDamage;

    [SerializeField] private Sprite defaultSprite; // Sprite mặc định khi không có cung (cho player)
    [SerializeField] private Sprite[] bowSprites; // Mảng sprite cho các cấp độ cung 1-5 (cho player)
    [SerializeField] private Sprite defaultSpriteWP; // Sprite mặc định cho UI
    [SerializeField] private Sprite[] bowSpritesWP; // Mảng sprite cho UI cấp độ cung 1-5
    [SerializeField] private Sprite defaultSpriteShop; // Sprite mặc định cho UI Shop
    [SerializeField] private Sprite[] bowSpritesShop; // Mảng sprite cho UI Shop cấp độ cung 1-5

    public TextMeshProUGUI BowPrice;
    public TextMeshProUGUI currentDamageTxt;
    public TextMeshProUGUI nextDamageTxt;

    [SerializeField] private GameObject changeToBowPanel;
    [SerializeField] private GameObject changeToBowPanel2;

    private float lastUpgradeTime = 0f;
    private float upgradeCooldown = 0.5f; // Cooldown để tránh gọi nhiều lần

    void Start()
    {
        if (damage.Length != upgradeCosts.Length + 1)
        {
            Debug.LogError("WP_BowManager configuration mismatch!");
        }
        if (BowPrice == null || currentDamageTxt == null || nextDamageTxt == null)
        {
            Debug.LogError("One or more TextMeshProUGUI fields not assigned in Inspector!");
        }
        if (bowSprites == null || bowSprites.Length != 5)
        {
            Debug.LogError("bowSprites not properly assigned! Need 5 sprites for levels 1-5.");
        }
        if (bowSpritesWP == null || bowSpritesWP.Length != 5)
        {
            Debug.LogError("bowSpritesWP not properly assigned! Need 5 sprites for levels 1-5.");
        }
        if (bowSpritesShop == null || bowSpritesShop.Length != 6)
        {
            Debug.LogError("bowSpritesShop not properly assigned! Need 6 sprites for levels 0-5.");
        }
        currentBowDamage = GetCurrentDamage();
        UpdateBowPriceUI();
        UpdateBowUI();
        Debug.Log($"Game started - Bow Level: {currentLevel}, Damage: {currentBowDamage}");
    }

    public float GetCurrentDamage()
    {
        return currentLevel < damage.Length ? damage[currentLevel] : damage[damage.Length - 1];
    }

    public Sprite GetCurrentBowSprite() // Dùng cho player sprite
    {
        if (currentLevel == 0)
            return defaultSprite;
        else
            return bowSprites[currentLevel - 1];
    }

    public Sprite GetCurrentBowSpriteWP() // Dùng cho UI
    {
        if (currentLevel == 0)
            return defaultSpriteWP;
        else
            return bowSpritesWP[currentLevel - 1];
    }

    public Sprite GetCurrentBowSpriteShop() // Dùng cho UI Shop
    {
        if (currentLevel == 0)
            return defaultSpriteShop;
        else
            return bowSpritesShop[currentLevel];
    }

    public bool HasBow()
    {
        return currentLevel > 0;
    }

    public bool UpgradeBow(PlayerManager playerManager)
    {
        if (Time.time - lastUpgradeTime < upgradeCooldown)
        {
            Debug.Log("Upgrade called too soon! Ignoring this call.");
            return false;
        }

        Debug.Log($"Before upgrade: Level = {currentLevel}, Damage = {GetCurrentDamage()}, Gold = {playerManager.gold}");

        if (currentLevel >= upgradeCosts.Length)
        {
            Debug.Log("Bow is already at max level!");
            UpdateBowPriceUI();
            return false;
        }

        int costIndex = currentLevel;
        if (playerManager.gold >= upgradeCosts[costIndex])
        {
            playerManager.gold -= upgradeCosts[costIndex];
            currentLevel++;
            currentBowDamage = GetCurrentDamage();
            lastUpgradeTime = Time.time;
            Debug.Log($"After upgrade: Level = {currentLevel}, Damage = {GetCurrentDamage()}, Gold = {playerManager.gold}");
            UpdateBowPriceUI();
            UpdateBowUI();
            UpdateBowUI2();
            return true;
        }
        else
        {
            Debug.Log($"Not enough gold to upgrade to Bow Level {currentLevel + 1}! Required: {upgradeCosts[costIndex]} gold, Available: {playerManager.gold}");
            return false;
        }
    }

    private void UpdateBowPriceUI()
    {
        if (BowPrice != null)
        {
            if (currentLevel >= upgradeCosts.Length)
            {
                BowPrice.text = "Max Level";
            }
            else
            {
                BowPrice.text = upgradeCosts[currentLevel].ToString();
            }
            Debug.Log($"BowPrice updated to: {BowPrice.text}");
        }

        if (currentDamageTxt != null)
        {
            currentDamageTxt.text = GetCurrentDamage().ToString();
            Debug.Log($"Current Damage UI updated to: {currentDamageTxt.text}");
        }

        if (nextDamageTxt != null)
        {
            if (currentLevel < damage.Length - 1)
            {
                nextDamageTxt.text = damage[currentLevel + 1].ToString();
            }
            else
            {
                nextDamageTxt.text = "Max";
            }
            Debug.Log($"Next Damage UI updated to: {nextDamageTxt.text}");
        }
    }

    private void UpdateBowUI()
    {
        if (changeToBowPanel != null)
        {
            var bowImage = changeToBowPanel.GetComponent<UnityEngine.UI.Image>();
            if (bowImage != null)
            {
                bowImage.sprite = GetCurrentBowSpriteWP();
                Debug.Log($"UpdateBowUI: Sprite updated to {bowImage.sprite.name} for Level {currentLevel}");
            }
            else
            {
                Debug.LogWarning("UpdateBowUI: Bow Image component not found on changeToBowPanel!");
            }
        }
        else
        {
            Debug.LogWarning("UpdateBowUI: changeToBowPanel is not assigned!");
        }
    }

    private void UpdateBowUI2()
    {
        if (changeToBowPanel2 != null)
        {
            var bowImage = changeToBowPanel2.GetComponent<UnityEngine.UI.Image>();
            if (bowImage != null)
            {
                bowImage.sprite = GetCurrentBowSpriteShop();
                Debug.Log($"UpdateBowUI2: Sprite updated to {bowImage.sprite.name} for Level {currentLevel}");
            }
            else
            {
                Debug.LogWarning("UpdateBowUI2: Bow Image component not found on changeToBowPanel2!");
            }
        }
        else
        {
            Debug.LogWarning("UpdateBowUI2: changeToBowPanel2 is not assigned!");
        }
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }
}