using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    PlayerManager playerManager;
    Mainmenu menu;
    AmorManager amorManager;
    WP_SwordManager swordManager;
    WP_BowManager bowManager;
    WP_AxeManager axeManager;

    void Start()
    {
        playerManager = FindFirstObjectByType<PlayerManager>();
        menu = FindFirstObjectByType<Mainmenu>();
        amorManager = FindFirstObjectByType<AmorManager>();
        swordManager = FindFirstObjectByType<WP_SwordManager>();
        bowManager = FindFirstObjectByType<WP_BowManager>();
        axeManager = FindFirstObjectByType<WP_AxeManager>();
        if (playerManager == null) Debug.LogError("PlayerManager not found!");
        if (menu == null) Debug.LogError("Mainmenu not found!");
        if (amorManager == null) Debug.LogError("AmorManager not found!");
        if (swordManager == null) Debug.LogError("WP_SwordManager not found!");
        if (bowManager == null) Debug.LogError("WP_BowManager not found!");
        if (axeManager == null) Debug.LogError("WP_AxeManager not found!");
    }

    public void PurchaseHealthPotion()
    {
        if (playerManager.gold >= 100)
        {
            playerManager.gold -= 100;
            menu.SetNoGold(playerManager.gold);
            Debug.Log("Health potion bought! Gold remaining: " + playerManager.gold);
        }
        else
        {
            Debug.Log("Not enough gold to buy health potion!");
        }
    }

    public void UpgradeArmor()
    {
        if (amorManager != null && playerManager != null)
        {
            if (amorManager.GetCurrentLevel() < 5)
            {
                if (amorManager.UpgradeArmor(playerManager))
                {
                    menu.SetNoGold(playerManager.gold);
                }
            }
            else if (amorManager.GetCurrentArmor() < amorManager.GetMaxArmor())
            {
                if (playerManager.gold >= 50)
                {
                    playerManager.gold -= 50;
                    amorManager.ResetArmor();
                    Debug.Log($"Armor restored to max: {amorManager.GetMaxArmor()}");
                    playerManager.ToggleArmor();
                    menu.SetNoGold(playerManager.gold);
                }
                else
                {
                    Debug.Log("Not enough gold to restore armor! Required: 50 gold");
                }
            }
            else
            {
                Debug.Log("Armor is already at max!");
            }
        }
        else
        {
            Debug.LogError("AmorManager or PlayerManager not initialized!");
        }
    }

    public void PurchaseSword()
    {
        if (swordManager != null && playerManager != null)
        {
            if (swordManager.GetCurrentLevel() < 6)
            {
                if (swordManager.UpgradeSword(playerManager))
                {
                    menu.SetNoGold(playerManager.gold);
                    playerManager.ToggleSword();
                    playerManager.ChangeToSword();
                }
            }
            else
            {
                Debug.Log("Sword is already at max level! Cannot purchase more.");
            }
        }
        else
        {
            Debug.LogError("WP_SwordManager or PlayerManager not initialized!");
        }
    }

    public void PurchaseBow()
    {
        if (bowManager != null && playerManager != null)
        {
            if (bowManager.GetCurrentLevel() < 6)
            {
                if (bowManager.UpgradeBow(playerManager))
                {
                    menu.SetNoGold(playerManager.gold);
                    playerManager.ToggleBow();
                    playerManager.ChangeToBow();
                }
            }
            else
            {
                Debug.Log("Bow is already at max level! Cannot purchase more.");
            }
        }
        else
        {
            Debug.LogError("WP_BowManager or PlayerManager not initialized!");
        }
    }

    public void PurchaseAxe()
    {
        if (axeManager != null && playerManager != null)
        {
            if (axeManager.GetCurrentLevel() < 6)
            {
                if (axeManager.UpgradeAxe(playerManager))
                {
                    menu.SetNoGold(playerManager.gold);
                    playerManager.ToggleAxe();
                    playerManager.ChangeToAxe();
                }
            }
            else
            {
                Debug.Log("Axe is already at max level! Cannot purchase more.");
            }
        }
        else
        {
            Debug.LogError("WP_AxeManager or PlayerManager not initialized!");
        }
    }
}