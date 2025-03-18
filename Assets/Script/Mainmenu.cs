using UnityEngine;
using TMPro;

public class Mainmenu : MonoBehaviour
{
    PlayerManager playerManager;
    public GameObject revivePanel;
    public GameObject startGamePanel;
    public GameObject ShopPanel;
    public GameObject resourcePanel;
    public GameObject Utility;

    public TMP_InputField playerNameInput;
    public TextMeshProUGUI NoGold;

    void Start()
    {
        playerManager = FindAnyObjectByType<PlayerManager>();
        if (revivePanel == null || startGamePanel == null || ShopPanel == null || resourcePanel == null || Utility == null || NoGold == null)
        {
            Debug.LogError("One or more panel references are not assigned in the Inspector!");
        }

        revivePanel.SetActive(false);
        startGamePanel.SetActive(true);
        ShopPanel.SetActive(false);
        resourcePanel.SetActive(true);
        Utility.SetActive(false);

        if (playerNameInput == null)
        {
            playerNameInput = startGamePanel.transform.Find("InputField (TMP)")?.GetComponent<TMP_InputField>();
            if (playerNameInput == null) Debug.LogError("InputField not found!");
        }

        if (playerManager != null)
        {
            SetNoGold(playerManager.gold);
        }
        else
        {
            Debug.LogWarning("PlayerManager not found in Start! Defaulting gold to 10000.");
            SetNoGold(10000);
        }
    }

    public void OpenShop()
    {
        Debug.Log("Opening Shop...");
        ShopPanel.SetActive(true);
        Debug.Log("ShopPanel active: " + ShopPanel.activeSelf + ", resourcePanel active: " + resourcePanel.activeSelf);
    }

    public void CloseShop()
    {
        ShopPanel.SetActive(false);
        Debug.Log("Shop closed, resourcePanel active: " + resourcePanel.activeSelf);
    }

    public void BuyUtility()
    {
        Debug.Log("Buying Health Potion...");
        if (Utility == null)
        {
            Debug.LogError("Utility is not assigned in the Inspector!");
            return;
        }
        if (ShopPanel != null && !ShopPanel.activeSelf)
        {
            ShopPanel.SetActive(true);
        }
        Utility.SetActive(true);
        Debug.Log("Utility active: " + Utility.activeSelf);

        var bloodButton = Utility.transform.Find("Blood");
        if (bloodButton != null)
        {
            Debug.Log("Blood button active: " + bloodButton.gameObject.activeSelf);
        }
        else
        {
            Debug.LogWarning("Blood button not found inside Utility! Please check the Hierarchy.");
        }
    }

    public void UserUtility()
    {
        Debug.Log("Using Health Potion...");
        if (Utility != null) Utility.SetActive(false);

       
        if (playerManager == null)
        {
            playerManager = FindAnyObjectByType<PlayerManager>();
            if (playerManager == null)
            {
                Debug.LogError("playerManager not found in UserUtility!");
                return;
            }
        }

        playerManager.FullHeal();
        Debug.Log("Player healed after using potion!");
        Debug.Log("Health Potion Panel active: " + (Utility != null ? Utility.activeSelf : "null"));
    }

    public void SetNoGold(int gold)
    {
        if (NoGold != null) NoGold.text = gold.ToString();
    }

    public void Revive()
    {
        if (revivePanel != null) revivePanel.SetActive(true);
        Time.timeScale = 0;
        Debug.Log("Revive panel opened");
    }

    public void startGame()
    {
        string playerName = playerNameInput != null ? playerNameInput.text : "Player";
        if (string.IsNullOrEmpty(playerName)) playerName = "Player";

        PlayerManager playerManager = FindAnyObjectByType<PlayerManager>();
        if (playerManager != null)
        {
            playerManager.SetPlayerName(playerName);
        }
        else
        {
            Debug.LogError("PlayerManager not found!");
        }

        if (revivePanel != null) revivePanel.SetActive(false);
        if (startGamePanel != null) startGamePanel.SetActive(false);
        Time.timeScale = 1;
        Debug.Log("Game started with player: " + playerName);
    }
}