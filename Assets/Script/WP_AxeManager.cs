using TMPro;
using UnityEngine;

public class WP_AxeManager : MonoBehaviour
{
    [SerializeField] private int currentLevel = 1; // Bắt đầu từ cấp 1 (mặc định có búa)
    private float[] damage = { 10f, 20f, 30f, 40f, 50f, 60f }; // 6 cấp độ sát thương (1-6)
    private int[] upgradeCosts = { 300, 600, 1200, 1800, 2400 }; // 5 lần nâng cấp từ 1->2, 2->3, ..., 5->6
    private float currentAxeDamage;

    [SerializeField] private Sprite[] axeSprites; // Mảng 6 sprite cho các cấp độ rìu 1-6 (cho player)
    [SerializeField] private Sprite[] axeSpritesWP; // Mảng 6 sprite cho UI cấp độ rìu 1-6
    [SerializeField] private Sprite[] axeSpritesShop; // Mảng 6 sprite cho UI Shop cấp độ rìu 1-6

    public TextMeshProUGUI AxePrice; // Giá nâng cấp
    public TextMeshProUGUI currentDamageTxt; // Sát thương hiện tại
    public TextMeshProUGUI nextDamageTxt; // Sát thương cấp tiếp theo

    [SerializeField] private GameObject changeToAxePanel; // Panel UI hiển thị rìu
    [SerializeField] private GameObject changeToAxePanel2; // Panel UI thứ hai (Shop UI)

    private float lastUpgradeTime = 0f;
    private float upgradeCooldown = 0.5f; // Cooldown để tránh gọi nhiều lần

    void Start()
    {
        if (damage.Length != upgradeCosts.Length + 1)
        {
            Debug.LogError("WP_AxeManager configuration mismatch!");
        }
        if (AxePrice == null || currentDamageTxt == null || nextDamageTxt == null)
        {
            Debug.LogError("One or more TextMeshProUGUI fields not assigned in Inspector!");
        }
        if (axeSprites == null || axeSprites.Length != 6)
        {
            Debug.LogError("axeSprites not properly assigned! Need 6 sprites for levels 1-6.");
        }
        if (axeSpritesWP == null || axeSpritesWP.Length != 6)
        {
            Debug.LogError("axeSpritesWP not properly assigned! Need 6 sprites for levels 1-6.");
        }
        if (axeSpritesShop == null || axeSpritesShop.Length != 6)
        {
            Debug.LogError("axeSpritesShop not properly assigned! Need 6 sprites for levels 1-6.");
        }

        currentAxeDamage = GetCurrentDamage();
        UpdateAxePriceUI();
        UpdateAxeUI();
        Debug.Log($"Game started - Axe Level: {currentLevel}, Damage: {currentAxeDamage}");
    }

    public float GetCurrentDamage()
    {
        return damage[currentLevel - 1]; // Truy cập dựa trên chỉ số từ 0
    }

    public Sprite GetCurrentAxeSprite() // Dùng cho player sprite
    {
        return axeSprites[currentLevel - 1];
    }

    public Sprite GetCurrentAxeSpriteWP() // Dùng cho UI (hiển thị cấp hiện tại)
    {
        return axeSpritesWP[currentLevel - 1];
    }

    public Sprite GetCurrentAxeSpriteShop() // Dùng cho UI Shop (hiển thị cấp tiếp theo)
    {
        if (currentLevel >= 6) // Nếu đã đạt cấp tối đa, trả về sprite cấp 6
            return axeSpritesShop[5];
        return axeSpritesShop[currentLevel]; // Trả về sprite của cấp tiếp theo
    }

    public bool HasAxe()
    {
        return true; // Luôn có rìu vì bắt đầu từ cấp 1
    }

    public bool UpgradeAxe(PlayerManager playerManager)
    {
        if (Time.time - lastUpgradeTime < upgradeCooldown)
        {
            Debug.Log("Upgrade called too soon! Ignoring this call.");
            return false;
        }

        Debug.Log($"Before upgrade: Level = {currentLevel}, Damage = {GetCurrentDamage()}, Gold = {playerManager.gold}");

        if (currentLevel >= 6) // Đã đạt cấp tối đa
        {
            Debug.Log("Axe is already at max level!");
            UpdateAxePriceUI();
            return false;
        }

        int costIndex = currentLevel - 1; // Vì bắt đầu từ cấp 1, chỉ số mảng bắt đầu từ 0
        if (playerManager.gold >= upgradeCosts[costIndex])
        {
            playerManager.gold -= upgradeCosts[costIndex];
            currentLevel++;
            currentAxeDamage = GetCurrentDamage();
            lastUpgradeTime = Time.time;
            Debug.Log($"After upgrade: Level = {currentLevel}, Damage = {GetCurrentDamage()}, Gold = {playerManager.gold}");
            UpdateAxePriceUI();
            UpdateAxeUI();
            UpdateAxeUI2();
            return true;
        }
        else
        {
            Debug.Log($"Not enough gold to upgrade to Axe Level {currentLevel + 1}! Required: {upgradeCosts[costIndex]} gold, Available: {playerManager.gold}");
            return false;
        }
    }

    private void UpdateAxePriceUI()
    {
        if (AxePrice != null)
        {
            if (currentLevel >= 6)
            {
                AxePrice.text = "Max Level";
            }
            else
            {
                AxePrice.text = upgradeCosts[currentLevel - 1].ToString();
            }
            Debug.Log($"AxePrice updated to: {AxePrice.text}");
        }

        if (currentDamageTxt != null)
        {
            currentDamageTxt.text = GetCurrentDamage().ToString();
            Debug.Log($"Current Damage UI updated to: {currentDamageTxt.text}");
        }

        if (nextDamageTxt != null)
        {
            if (currentLevel < 6)
            {
                nextDamageTxt.text = damage[currentLevel].ToString();
            }
            else
            {
                nextDamageTxt.text = "Max";
            }
            Debug.Log($"Next Damage UI updated to: {nextDamageTxt.text}");
        }
    }

    private void UpdateAxeUI()
    {
        if (changeToAxePanel != null)
        {
            var axeImage = changeToAxePanel.GetComponent<UnityEngine.UI.Image>();
            if (axeImage != null)
            {
                axeImage.sprite = GetCurrentAxeSpriteWP();
                Debug.Log($"UpdateAxeUI: Sprite updated to {axeImage.sprite.name} for Level {currentLevel}");
            }
            else
            {
                Debug.LogWarning("UpdateAxeUI: Axe Image component not found on changeToAxePanel!");
            }
        }
        else
        {
            Debug.LogWarning("UpdateAxeUI: changeToAxePanel is not assigned!");
        }
    }

    private void UpdateAxeUI2()
    {
        if (changeToAxePanel2 != null)
        {
            var axeImage = changeToAxePanel2.GetComponent<UnityEngine.UI.Image>();
            if (axeImage != null)
            {
                axeImage.sprite = GetCurrentAxeSpriteShop();
                Debug.Log($"UpdateAxeUI2: Sprite updated to {axeImage.sprite.name} for Next Level {currentLevel + 1}");
            }
            else
            {
                Debug.LogWarning("UpdateAxeUI2: Axe Image component not found on changeToAxePanel2!");
            }
        }
        else
        {
            Debug.LogWarning("UpdateAxeUI2: changeToAxePanel2 is not assigned!");
        }
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }
}