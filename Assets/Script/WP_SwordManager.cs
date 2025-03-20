using TMPro;
using UnityEngine;

public class WP_SwordManager : MonoBehaviour
{
    [SerializeField] private int currentLevel = 0; // 0 = không có kiếm, 1-5 = cấp độ kiếm
    private float[] damage = { 0f, 15f, 25f, 35f, 45f, 55f, 65f }; // damage[0] = 0 (không có kiếm), damage[1-5] = damage theo cấp
    private int[] upgradeCosts = { 200, 500, 1000, 1500, 2000, 2500 }; // Costs để mua/nâng cấp: 0->1, 1->2, 2->3, 3->4, 4->5
    private float currentSword;

    [SerializeField] private Sprite defaultSprite; // Sprite mặc định khi không có kiếm (cho player)
    [SerializeField] private Sprite[] swordSprites; // Mảng 5 sprite cho các cấp độ kiếm 1-5 (cho player)
    [SerializeField] private Sprite defaultSpriteWP; // Sprite mặc định cho UI
    [SerializeField] private Sprite[] swordSpritesWP; // Mảng 5 sprite cho UI cấp độ kiếm 1-5

    [SerializeField] private Sprite defaultSpriteShop; // Sprite mặc định cho UI
    [SerializeField] private Sprite[] swordSpritesShop; // Mảng 5 sprite cho UI cấp độ kiếm 1-5

    public TextMeshProUGUI SwordPrice;
    public TextMeshProUGUI currentdamagetxt;
    public TextMeshProUGUI nextdamagetxt;

    [SerializeField] private GameObject changeToSwordPanel;
    [SerializeField] private GameObject changeToSwordPanel2;

    private float lastUpgradeTime = 0f;
    private float upgradeCooldown = 0.5f; // Cooldown 0.5 giây để tránh gọi nhiều lần

    void Start()
    {
        if (damage.Length != upgradeCosts.Length + 1)
        {
            Debug.LogError("WP_SwordManager configuration mismatch!");
        }
        if (SwordPrice == null || currentdamagetxt == null || nextdamagetxt == null)
        {
            Debug.LogError("One or more TextMeshProUGUI fields not assigned in Inspector!");
        }
        if (swordSprites == null || swordSprites.Length != 5)
        {
            Debug.LogError("swordSprites not properly assigned! Need 5 sprites for levels 1-5.");
        }
        if (swordSpritesWP == null || swordSpritesWP.Length != 5)
        {
            Debug.LogError("swordSpritesWP not properly assigned! Need 5 sprites for levels 1-5.");
        }
        if (swordSpritesShop == null || swordSpritesShop.Length != 6)
        {
            Debug.LogError("swordSpritesWP not properly assigned! Need 5 sprites for levels 1-5.");
        }
        currentSword = GetCurrentDamage();
        UpdateSwordPriceUI();
        UpdateSwordUI(); // Cập nhật UI ngay khi khởi tạo
        Debug.Log($"Game started - Level: {currentLevel}, Damage: {currentSword}");
    }

    public float GetCurrentDamage()
    {
        return currentLevel < damage.Length ? damage[currentLevel] : damage[damage.Length - 1];
    }

    public Sprite GetCurrentSwordSprite() // Dùng cho player sprite
    {
        if (currentLevel == 0)
            return defaultSprite;
        else
            return swordSprites[currentLevel - 1];
    }

    public Sprite GetCurrentSwordSpriteWP() // Dùng cho UI
    {
        if (currentLevel == 0)
            return defaultSpriteWP;
        else
            return swordSpritesWP[currentLevel - 1];
    }

    public Sprite GetCurrentSwordSpriteshop() // Dùng cho UI
    {
        if (currentLevel == 0)
            return defaultSpriteWP;
        else
            return swordSpritesShop[currentLevel];
    }

    public bool HasSword()
    {
        return currentLevel > 0;
    }

    public bool UpgradeSword(PlayerManager playerManager)
    {
        if (Time.time - lastUpgradeTime < upgradeCooldown)
        {
            Debug.Log("Upgrade called too soon! Ignoring this call.");
            return false;
        }

        Debug.Log($"Before upgrade: Level = {currentLevel}, Damage = {GetCurrentDamage()}, Gold = {playerManager.gold}");

        if (currentLevel >= upgradeCosts.Length)
        {
            Debug.Log("Sword is already at max level!");
            UpdateSwordPriceUI();
            return false;
        }

        int costIndex = currentLevel;
        if (playerManager.gold >= upgradeCosts[costIndex])
        {
            playerManager.gold -= upgradeCosts[costIndex];
            currentLevel++;
            currentSword = GetCurrentDamage();
            lastUpgradeTime = Time.time;
            Debug.Log($"After upgrade: Level = {currentLevel}, Damage = {GetCurrentDamage()}, Gold = {playerManager.gold}");
            UpdateSwordPriceUI();
            UpdateSwordUI(); // Cập nhật UI sau khi nâng cấp
            UpdateSwordUI2();
            return true;
        }
        else
        {
            Debug.Log($"Not enough gold to upgrade to Sword Level {currentLevel + 1}! Required: {upgradeCosts[costIndex]} gold, Available: {playerManager.gold}");
            return false;
        }
    }

    private void UpdateSwordPriceUI()
    {
        if (SwordPrice != null)
        {
            if (currentLevel >= upgradeCosts.Length)
            {
                SwordPrice.text = "Max Level";
            }
            else
            {
                SwordPrice.text = upgradeCosts[currentLevel].ToString();
            }
            Debug.Log($"SwordPrice updated to: {SwordPrice.text}");
        }

        if (currentdamagetxt != null)
        {
            currentdamagetxt.text = GetCurrentDamage().ToString();
            Debug.Log($"Current Damage UI updated to: {currentdamagetxt.text}");
        }

        if (nextdamagetxt != null)
        {
            if (currentLevel < damage.Length - 1)
            {
                nextdamagetxt.text = damage[currentLevel + 1].ToString();
            }
            else
            {
                nextdamagetxt.text = "Max";
            }
            Debug.Log($"Next Damage UI updated to: {nextdamagetxt.text}");
        }
    }

    private void UpdateSwordUI()
    {
        if (changeToSwordPanel != null)
        {
            var swordImage = changeToSwordPanel.GetComponent<UnityEngine.UI.Image>();
            if (swordImage != null)
            {
                swordImage.sprite = GetCurrentSwordSpriteWP();
                Debug.Log($"UpdateSwordUI: Sprite updated to {swordImage.sprite.name} for Level {currentLevel}");
            }
            else
            {
                Debug.LogWarning("UpdateSwordUI: Sword Image component not found on changeToSwordPanel!");
            }
        }
        else
        {
            Debug.LogWarning("UpdateSwordUI: changeToSwordPanel is not assigned!");
        }
    }

    private void UpdateSwordUI2()
    {
        if (changeToSwordPanel2 != null)
        {
            var swordImage = changeToSwordPanel2.GetComponent<UnityEngine.UI.Image>();
            if (swordImage != null)
            {
                swordImage.sprite = GetCurrentSwordSpriteshop();
                Debug.Log($"UpdateSwordUI: Sprite updated to {swordImage.sprite.name} for Level {currentLevel}");
            }
            else
            {
                Debug.LogWarning("UpdateSwordUI: Sword Image component not found on changeToSwordPanel!");
            }
        }
        else
        {
            Debug.LogWarning("UpdateSwordUI: changeToSwordPanel is not assigned!");
        }
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }
}