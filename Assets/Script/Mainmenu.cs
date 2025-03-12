using UnityEngine;
using TMPro; // Required for TextMeshPro

public class Mainmenu : MonoBehaviour
{
    public GameObject revivePanel;
    public GameObject startGamePanel;
    public TMP_InputField playerNameInput; // Reference to the InputField

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        revivePanel.SetActive(false);
        startGamePanel.SetActive(true);

        // Automatically find the InputField if not assigned in Inspector
        if (playerNameInput == null)
        {
            playerNameInput = startGamePanel.transform.Find("InputField (TMP)").GetComponent<TMP_InputField>();
            if (playerNameInput == null) Debug.LogError("InputField not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Revive()
    {
        revivePanel.SetActive(true); // Show Game Over UI
        Time.timeScale = 0; // Pause the game
        Debug.Log("Ngu");
    }

    public void startGame()
    {
        // Get the text from the InputField
        string playerName = playerNameInput.text;
        if (string.IsNullOrEmpty(playerName)) playerName = "Player"; // Default name if empty

        // Find the PlayerManager and set the name
        PlayerManager playerManager = FindAnyObjectByType<PlayerManager>();
        if (playerManager != null)
        {
            playerManager.SetPlayerName(playerName); // Call new method in PlayerManager
        }
        else
        {
            Debug.LogError("PlayerManager not found!");
        }

        revivePanel.SetActive(false); // Hide revive panel
        startGamePanel.SetActive(false); // Hide start panel
        Time.timeScale = 1; // Resume game
        Debug.Log("Thong minh - Game started with player: " + playerName);
    }
}