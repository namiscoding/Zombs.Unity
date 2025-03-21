
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int wood;          // Số lượng gỗ hiện tại
    [SerializeField] private int stone;         // Số lượng đá hiện tại
    [SerializeField] private Text txtWood;      // Text UI hiển thị số lượng gỗ
    [SerializeField] private Text txtStone;     // Text UI hiển thị số lượng đá
    [SerializeField] private GameObject NightUI; //Night UI panel
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

    void Start()
    {
        UpdateResourceUI();
    }
    //void Update()
    //{
    //    // Update time cycle progress
    //    timeElapsed += Time.deltaTime;

    //    if (timeElapsed >= timeDuration) // Reset cycle when it completes
    //    {
    //        timeElapsed = 0f;
    //    }

    //    // Update the UI bar to reflect time progression
    //    float barFillAmount = timeElapsed / timeDuration;
    //    nightBar.fillAmount = barFillAmount;
    //}
    void UpdateResourceUI()
    {
        txtWood.text = "" + wood;
        txtStone.text = "" + stone;
        //txtWood.text = "Wood: " + wood;
        //txtStone.text = "Stone: " + stone;
    }

    // Thêm đá
    public void AddStone(int quality)
    {
        stone += quality;      // Cộng số lượng đá
        UpdateResourceUI();    // Cập nhật lại UI
    }

    // Thêm gỗ
    public void AddWood(int quality)
    {
        wood += quality;       // Cộng số lượng gỗ
        UpdateResourceUI();    // Cập nhật lại UI
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
        {            yield return new WaitForSeconds(timeDuration - 5f); // Daytime duration (55 seconds)

            // 🚨 Show Countdown Warning ONLY before night
            for (int i = 5; i > 0; i--)
            {
                warningText.text = "Night starts in " + i + " seconds!";
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
