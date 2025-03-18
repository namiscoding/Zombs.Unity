using UnityEngine;

public class ShopManager : MonoBehaviour
{
    PlayerManager playerManager;
    Mainmenu menu;

    void Start()
    {
        playerManager = FindFirstObjectByType<PlayerManager>();
        menu = FindFirstObjectByType<Mainmenu>();
        if (playerManager == null) Debug.LogError("PlayerManager not found!");
        if (menu == null) Debug.LogError("Mainmenu not found!");
    }

    public void PurchaseHealthPotion()
    {
        if (playerManager == null || menu == null)
        {
            Debug.LogError("PlayerManager or Mainmenu not properly initialized!");
            return;
        }

        if (playerManager.gold >= 100)
        {
            playerManager.gold -= 100;
            menu.SetNoGold(playerManager.gold);
            //menu.BuyUtility();
            Debug.Log("Health potion bought! Gold remaining: " + playerManager.gold);
        }
        else
        {
            Debug.Log("Not enough gold to buy health potion!");
        }
    }
}