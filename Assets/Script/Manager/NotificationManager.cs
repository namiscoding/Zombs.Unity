using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; // For TextMeshProUGUI

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    [SerializeField] private GameObject notificationPrefab; // Prefab for the notification UI (parent with a TextMeshProUGUI child)
    private float displayDuration = 2f; // Duration each notification is displayed (in seconds)
    private float fadeDuration = 0.3f; // Duration for fade in/out effect (in seconds)
    private float verticalOffset = 40f; // Offset from the top of the screen (in pixels)

    private Canvas canvas; // Reference to the Canvas
    private GameObject currentNotification; // Reference to the currently displayed notification
    private Coroutine currentCoroutine; // Reference to the current display coroutine

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        // Find the Canvas (same as BuildingInfoPanel)
        BuildingInfoPanel panel = FindFirstObjectByType<BuildingInfoPanel>();
        if (panel != null)
        {
            canvas = panel.GetComponentInParent<Canvas>();
        }
        if (canvas == null)
        {
            Debug.LogError("No Canvas found for NotificationManager!");
            return;
        }
    }

    // Public method to show a notification
    public void ShowNotification(string message)
    {
        // If a notification is currently displayed, destroy it and stop its coroutine
        if (currentNotification != null)
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            Destroy(currentNotification);
            currentNotification = null;
        }

        // Display the new notification
        currentCoroutine = StartCoroutine(DisplayNotification(message));
    }

    private IEnumerator DisplayNotification(string message)
    {
        // Create a new notification instance
        currentNotification = Instantiate(notificationPrefab, canvas.transform);
        // Find the TextMeshProUGUI component on the child "NotificationText"
        TextMeshProUGUI notificationText = currentNotification.GetComponentInChildren<TextMeshProUGUI>();
        if (notificationText == null)
        {
            Debug.LogError("Notification prefab does not have a TextMeshProUGUI component on its child!");
            Destroy(currentNotification);
            yield break;
        }

        notificationText.text = message;

        RectTransform backgroundRect = notificationText.GetComponentInParent<Image>().rectTransform;
        Canvas.ForceUpdateCanvases();
        backgroundRect.sizeDelta = new Vector2(notificationText.preferredWidth + 60f, notificationText.preferredHeight + 50f);

        // Position the notification near the top of the screen
        RectTransform rectTransform = currentNotification.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 1f); // Anchor to top center
        rectTransform.anchorMax = new Vector2(0.5f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 1f); // Pivot at top center
        rectTransform.anchoredPosition = new Vector2(0, -verticalOffset); // Offset from the top

        // Fade in
        CanvasGroup canvasGroup = currentNotification.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = currentNotification.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Display for the specified duration
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;

        // Destroy the notification
        Destroy(currentNotification);
        currentNotification = null;
        currentCoroutine = null;
    }
}