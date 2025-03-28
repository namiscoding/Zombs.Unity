using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private Center center;
    public int CenterLevel => center != null ? center.GetLevel() : 0;
    public bool HasCenter => center != null;
    
    // Add reference to the enemy spawner
    private EnemySpawner enemySpawner;

    [SerializeField] private GameObject NightUI;
    [SerializeField] private Text warningText;
    [SerializeField] private Image nightBar;
    [SerializeField] private Image dayBar;
    [SerializeField] private Image nightBar2;
    [SerializeField] private Image dayBar2;
    [SerializeField] private float timeDuration = 60f;
    private bool isNightActive = false;
    private bool isNightCycleRunning = false;
    private bool isTimeBarRunning = false;
    private float timeElapsed = 0f; // Track the current time in cycle

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        // Find the enemy spawner in the scene
        enemySpawner = FindObjectOfType<EnemySpawner>();
        if (enemySpawner == null)
        {
            Debug.LogError("EnemySpawner not found in the scene!");
        }
    }

    void Start()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetCenter(Center centerInstance)
    {
        if (center == null)
        {
            center = centerInstance;
            // Trigger enemy spawning when base is placed
            if (enemySpawner != null)
            {
                enemySpawner.StartSpawning();
                Debug.Log("Center built, enemy spawning started!");
            }
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over! Center destroyed.");
        // Stop enemy spawning when game is over
        if (enemySpawner != null)
        {
            enemySpawner.StopSpawning();
        }
        // Implement game over logic (e.g., scene reload)
    }

    public void StartNightCycle()
    {
        if (!isNightCycleRunning) // Run only if it's not already running
        {
            isNightCycleRunning = true;
            StartCoroutine(ToggleNightCycle());
        }
    }

    IEnumerator ToggleNightCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeDuration - 5f); // Daytime duration (55 seconds)

            // 🚨 Show Countdown Warning ONLY before night
            for (int i = 5; i > 0; i--)
            {
                NotificationManager.Instance.ShowNotification($"Night starts in " + i + " seconds!");
                warningText.gameObject.SetActive(true);
                yield return new WaitForSeconds(1f); // Wait 1 second for countdown
            }

            // 🌙 Activate Night
            isNightActive = true;
            NightUI.SetActive(true);
            warningText.gameObject.SetActive(false); // Hide warning after night starts

            //a feature of spawn enemy will be here.
            yield return new WaitForSeconds(timeDuration); // Night duration (60 seconds)

            // ☀️ Switch to Daytime
            isNightActive = false;
            NightUI.SetActive(false);
        }
    }

    public void StartTimeBar()
    {
        if (!isTimeBarRunning) // Run only if it's not already running
        {
            isTimeBarRunning = true;
            StartCoroutine(ToggleTimeBar());
        }
    }

    IEnumerator ToggleTimeBar()
    {
        while (true)
        {
            timeElapsed = 0f; // Reset at the start of a new cycle

            // Process 1: Fill night bar (0 → 1), disable day bar
            dayBar.gameObject.SetActive(false);  // Hide day bar
            nightBar.gameObject.SetActive(true); // Show night bar
            dayBar2.gameObject.SetActive(false);  // Hide day bar
            nightBar2.gameObject.SetActive(true); // Show night bar

            while (timeElapsed < timeDuration)
            {
                timeElapsed += Time.deltaTime;

                // Night bar fills up
                float barFillAmount = timeElapsed / timeDuration;
                nightBar.fillAmount = barFillAmount;
                float barFillAmount2 = 1 - (timeElapsed / timeDuration);
                nightBar2.fillAmount = barFillAmount2;
                yield return null; // Wait for the next frame
            }; // Short delay before switching

            timeElapsed = 0f; // Reset for the next phase
            // Process 2: Empty night bar, Fill day bar (1 → 0)
            nightBar.gameObject.SetActive(false); // Hide night bar
            dayBar.gameObject.SetActive(true);   // Show day bar
            nightBar2.gameObject.SetActive(false); // Hide night bar
            dayBar2.gameObject.SetActive(true);

            while (timeElapsed < timeDuration)
            {
                timeElapsed += Time.deltaTime;

                // Day bar empties from 1 → 0
                float barFillAmount = 1 - (timeElapsed / timeDuration);
                dayBar.fillAmount = barFillAmount;
                float barFillAmount2 = timeElapsed / timeDuration;
                dayBar2.fillAmount = barFillAmount2;
                yield return null; // Wait for the next frame
            } // Short delay before restarting
        }
    }
}