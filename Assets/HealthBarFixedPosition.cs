        
using UnityEngine;

public class HealthBarFixedPosition : MonoBehaviour
{
    public RectTransform healthBarRectTransform;  // Reference to the Health Bar's RectTransform
    public Transform enemyTransform;  // The transform of the Enemy that the health bar should follow

    private Camera mainCamera;

    void Start()
    {
        // Get the main camera to convert world position to screen position
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Ensure the enemy exists
        if (enemyTransform != null && healthBarRectTransform != null)
        {
            // Convert enemy world position to screen position
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(enemyTransform.position);

            // Set health bar position (you can adjust the Y offset to position the bar above the enemy)
            healthBarRectTransform.position = screenPosition + new Vector3(0, 50, 0); // 50 is the offset (adjust as needed)
        }
    }
}