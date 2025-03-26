using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // For Slider

public abstract class Building : MonoBehaviour
{
    public BuildingData data;
    protected int currentLevel = 1;
    [HideInInspector]
    public int currentHealth;
    private SpriteRenderer spriteRenderer; // Cache the SpriteRenderer for efficiency
    private GameObject healthBarInstance; // Health bar UI instance
    private Slider healthBarSlider; // Slider component of the health bar
    private RectTransform healthBarRectTransform; // RectTransform for positioning
    private Canvas canvas; // Reference to the Canvas
    private Camera mainCamera; // Reference to the main camera
    private int maxHealth; // Store the maximum health for health bar calculation

    [SerializeField] private GameObject healthBarPrefab; // Assign the HealthBar prefab in the Inspector
    private float healthBarHeight = 30f; // Increased default height of the health bar
    private float healthBarOffset = 0.2f; // Small offset from the bottom of the sprite
    private float widthPadding = 0.9f; // Padding to ensure the health bar fits within the collider

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite(); // Set the initial sprite for Level 1
        UpdateStats();

        // Initialize the health bar
        InitializeHealthBar();
    }

    protected virtual void InitializeHealthBar()
    {
        if (healthBarPrefab == null)
        {
            Debug.LogError("HealthBarPrefab is not assigned in the Inspector!");
            return;
        }

        // Find the Canvas (same as BuildingInfoPanel)
        BuildingInfoPanel panel = FindFirstObjectByType<BuildingInfoPanel>();
        if (panel != null)
        {
            canvas = panel.GetComponentInParent<Canvas>();
        }
        if (canvas == null)
        {
            Debug.LogError("No Canvas found for health bar!");
            return;
        }

        // Find the main camera
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("No main camera found!");
            return;
        }

        // Instantiate the health bar
        healthBarInstance = Instantiate(healthBarPrefab, canvas.transform);
        healthBarSlider = healthBarInstance.GetComponent<Slider>();
        healthBarRectTransform = healthBarInstance.GetComponent<RectTransform>();

        if (healthBarSlider == null || healthBarRectTransform == null)
        {
            Debug.LogError("HealthBarPrefab does not have a Slider or RectTransform component!");
            return;
        }

        // Disable interactivity to prevent dragging
        healthBarSlider.interactable = false;

        // Store the maximum health
        maxHealth = currentHealth;

        // Initially hide the health bar (full health)
        UpdateHealthBar();
    }

    protected virtual void Update()
    {
        // Update the health bar position and scale every frame
        if (healthBarInstance != null && healthBarRectTransform != null)
        {
            PositionHealthBar();
        }

        // Debug: Press T to take damage, R to restore full health, H to heal 10
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10); // Deal 10 damage
            Debug.Log($"{data.buildingName} took 10 damage. Current health: {currentHealth}/{maxHealth}");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentHealth = maxHealth; // Restore full health
            UpdateHealthBar();
            Debug.Log($"{data.buildingName} restored to full health. Current health: {currentHealth}/{maxHealth}");
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(10); // Heal 10 health
            Debug.Log($"{data.buildingName} healed 10 health. Current health: {currentHealth}/{maxHealth}");
        }
    }

    protected virtual void OnDestroy()
    {
        // Destroy the health bar when the building is destroyed
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance);
            healthBarInstance = null; // Clear the reference
        }
    }

    public virtual void LevelUp()
    {
        if (currentLevel >= 6 || (currentLevel >= GameManager.Instance.CenterLevel && data is not CenterData))
        {
            NotificationManager.Instance.ShowNotification("deo nang cap duoc");
            return;
        }
        int nextLevelIndex = currentLevel - 1;
        if (ResourceManager.Instance.CanAfford(data.levelUpCosts[nextLevelIndex]))
        {
            ResourceManager.Instance.SpendResources(data.levelUpCosts[nextLevelIndex]);
            currentLevel++;
            UpdateSprite(); // Update the sprite for the new level
            UpdateStats();
            UpdateHealthBar(); // Update the health bar after leveling up
            UpdatePanelIfDisplayed(); // Update the panel to reflect the new level
        }
    }

    protected virtual void UpdateStats()
    {
        currentHealth = (int)(data.maxHealth * data.healthMultipliers[currentLevel - 1]);
        maxHealth = currentHealth; // Update maxHealth when stats change
        NotificationManager.Instance.ShowNotification($"{currentHealth}");
    }

    protected virtual void UpdateSprite()
    {
        if (spriteRenderer != null && data != null && data.sprites != null && data.sprites.Length >= currentLevel)
        {
            Sprite newSprite = data.sprites[currentLevel - 1];
            if (newSprite == null)
            {
                Debug.LogWarning($"Sprite for level {currentLevel} is null in {data.buildingName}!");
            }
            spriteRenderer.sprite = newSprite;
        }
    }

    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;
        UpdateHealthBar(); // Update the health bar when taking damage
        if (currentHealth <= 0)
        {
            // Notify BuildingManager to decrement the count before destroying
            BuildingManager buildingManager = FindObjectOfType<BuildingManager>();
            if (buildingManager != null)
            {
                buildingManager.OnBuildingSold(this);
            }
            Destroy(gameObject);
        }
    }

    public virtual void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        UpdateHealthBar();
    }

    void OnMouseDown()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10); // Deal 10 damage
            Debug.Log($"{data.buildingName} took 10 damage. Current health: {currentHealth}/{maxHealth}");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentHealth = maxHealth; // Restore full health
            UpdateHealthBar();
            Debug.Log($"{data.buildingName} restored to full health. Current health: {currentHealth}/{maxHealth}");
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(10); // Heal 10 health
            Debug.Log($"{data.buildingName} healed 10 health. Current health: {currentHealth}/{maxHealth}");
        }
    }

    private void UpdatePanelIfDisplayed()
    {
        BuildingInfoPanel panel = FindFirstObjectByType<BuildingInfoPanel>();
        if (panel != null && panel.currentBuilding == this)
        {
            panel.UpdatePanel();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarSlider == null) return;

        // Update the health bar value (normalized between 0 and 1)
        float healthPercentage = (float)currentHealth / maxHealth;
        healthBarSlider.value = healthPercentage;

        // Show the health bar if health is less than max, hide it if at full health
        healthBarInstance.SetActive(currentHealth < maxHealth);
    }

    private void PositionHealthBar()
    {
        if (mainCamera == null || canvas == null || spriteRenderer == null || spriteRenderer.sprite == null) return;

        // Get the building's world position
        Vector3 buildingWorldPos = transform.position;

        // Get the sprite's bounds to determine its height (for positioning)
        Bounds spriteBounds = spriteRenderer.sprite.bounds;
        float spriteHeight = spriteBounds.size.y * transform.localScale.y; // Account for sprite scaling

        // Calculate the position inside the sprite, near the bottom
        // The bottom of the sprite in world space is at buildingWorldPos.y - spriteHeight/2
        // Place the health bar just above the bottom edge (inside the sprite)
        float yOffsetInsideSprite = -spriteHeight / 2 + healthBarOffset; // Small offset from the bottom
        Vector3 offset = new Vector3(0, yOffsetInsideSprite, 0);
        Vector3 worldPosInsideSprite = buildingWorldPos + offset;

        // Convert world position to screen position
        Vector2 screenPos = mainCamera.WorldToScreenPoint(worldPosInsideSprite);

        // Convert screen position to Canvas position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
            out Vector2 localPoint
        );

        // Get the collider's width to determine the health bar's width
        float colliderWidth = 1f; // Default width if no collider is found
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider != null)
        {
            colliderWidth = boxCollider.size.x * transform.localScale.x; // Account for collider scaling
        }
        else
        {
            Debug.LogWarning($"{data.buildingName} has no BoxCollider2D or CircleCollider2D! Using default width for health bar.");
        }

        // Convert collider width from world units to screen units
        Vector3 colliderWorldRightEdge = buildingWorldPos + new Vector3(colliderWidth / 2, 0, 0);
        Vector3 colliderWorldLeftEdge = buildingWorldPos - new Vector3(colliderWidth / 2, 0, 0);
        Vector2 screenRightEdge = mainCamera.WorldToScreenPoint(colliderWorldRightEdge);
        Vector2 screenLeftEdge = mainCamera.WorldToScreenPoint(colliderWorldLeftEdge);
        float screenWidth = Mathf.Abs(screenRightEdge.x - screenLeftEdge.x);

        // Convert screen width to Canvas space using the Canvas's actual scale
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float canvasScaleFactor = canvasRect.lossyScale.x; // Use the Canvas's x-scale
        float healthBarWidth = screenWidth * canvasScaleFactor * widthPadding; // Apply padding to fit within collider

        // Set the health bar size
        healthBarRectTransform.sizeDelta = new Vector2(healthBarWidth, healthBarHeight);

        // Set the health bar's position (centered horizontally, positioned at the calculated y)
        healthBarRectTransform.anchoredPosition = localPoint;
    }

    public int GetLevel() => currentLevel;
}