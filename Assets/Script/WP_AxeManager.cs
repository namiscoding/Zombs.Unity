using TMPro;
using UnityEngine;

public class WP_AxeManager : MonoBehaviour
{
    [SerializeField] private int currentLevel = 1;
    private float[] damage = { 10f, 20f, 30f, 40f, 50f, 60f };
    private float[] collect = { 2, 4, 6, 8, 10, 12 };
    private int[] upgradeCosts = { 300, 600, 1200, 1800, 2400 };
    private float currentAxeDamage;

    [SerializeField] private Sprite[] axeSprites;
    [SerializeField] private Sprite[] axeSpritesWP;
    [SerializeField] private Sprite[] axeSpritesShop;

    public TextMeshProUGUI AxePrice;
    public TextMeshProUGUI currentDamageTxt;
    public TextMeshProUGUI nextDamageTxt;
    public TextMeshProUGUI currentCollectTxt;  // Thêm để hiển thị collect hiện tại
    public TextMeshProUGUI nextCollectTxt;     // Thêm để hiển thị collect tiếp theo

    [SerializeField] private GameObject changeToAxePanel;
    [SerializeField] private GameObject changeToAxePanel2;

    private float lastUpgradeTime = 0f;
    private float upgradeCooldown = 0.5f;

    void Start()
    {
        if (damage.Length != upgradeCosts.Length + 1 || collect.Length != damage.Length)
        {
            Debug.LogError("WP_AxeManager configuration mismatch! Damage, collect and upgradeCosts arrays must align properly.");
        }
        if (AxePrice == null || currentDamageTxt == null || nextDamageTxt == null ||
            currentCollectTxt == null || nextCollectTxt == null)
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
        Debug.Log($"Game started - Axe Level: {currentLevel}, Damage: {currentAxeDamage}, Collect: {GetCurrentCollect()}");
    }

    public float GetCurrentDamage()
    {
        return damage[currentLevel - 1];
    }

    public float GetCurrentCollect()
    {
        return collect[currentLevel - 1];
    }

    public Sprite GetCurrentAxeSprite()
    {
        return axeSprites[currentLevel - 1];
    }

    public Sprite GetCurrentAxeSpriteWP()
    {
        return axeSpritesWP[currentLevel - 1];
    }

    public Sprite GetCurrentAxeSpriteShop()
    {
        if (currentLevel >= 6)
            return axeSpritesShop[5];
        return axeSpritesShop[currentLevel];
    }

    public bool HasAxe()
    {
        return true;
    }

    public bool UpgradeAxe(PlayerManager playerManager)
    {
        if (Time.time - lastUpgradeTime < upgradeCooldown)
        {
            Debug.Log("Upgrade called too soon! Ignoring this call.");
            return false;
        }

        Debug.Log($"Before upgrade: Level = {currentLevel}, Damage = {GetCurrentDamage()}, Collect = {GetCurrentCollect()}, Gold = {ResourceManager.Instance.gold}");

        if (currentLevel >= 6)
        {
            Debug.Log("Axe is already at max level!");
            UpdateAxePriceUI();
            return false;
        }

        int costIndex = currentLevel - 1;
        if (ResourceManager.Instance.gold >= upgradeCosts[costIndex])
        {
            ResourceManager.Instance.UseGold(upgradeCosts[costIndex]);
            currentLevel++;
            currentAxeDamage = GetCurrentDamage();
            lastUpgradeTime = Time.time;
            Debug.Log($"After upgrade: Level = {currentLevel}, Damage = {GetCurrentDamage()}, Collect = {GetCurrentCollect()}, Gold = {ResourceManager.Instance.gold}");
            UpdateAxePriceUI();
            UpdateAxeUI();
            UpdateAxeUI2();
            return true;
        }
        else
        {
            Debug.Log($"Not enough gold to upgrade to Axe Level {currentLevel + 1}! Required: {upgradeCosts[costIndex]} gold, Available: {ResourceManager.Instance.gold}");
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

        // Cập nhật UI cho collect hiện tại
        if (currentCollectTxt != null)
        {
            currentCollectTxt.text = GetCurrentCollect().ToString();
            Debug.Log($"Current Collect UI updated to: {currentCollectTxt.text}");
        }

        // Cập nhật UI cho collect tiếp theo
        if (nextCollectTxt != null)
        {
            if (currentLevel < 6)
            {
                nextCollectTxt.text = collect[currentLevel].ToString();
            }
            else
            {
                nextCollectTxt.text = "Max";
            }
            Debug.Log($"Next Collect UI updated to: {nextCollectTxt.text}");
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