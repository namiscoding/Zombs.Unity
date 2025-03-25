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

        // Store the maximum health
        maxHealth = currentHealth;

        // Initially hide the health bar (full health)
        UpdateHealthBar();
    }

    protected virtual void Update()
    {
        // Update the health bar position every frame
        if (healthBarInstance != null && healthBarRectTransform != null)
        {
            PositionHealthBar();
        }
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
    }

    protected virtual void OnDestroy()
    {
        // Destroy the health bar when the building is destroyed
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance);
        }
    }

    public virtual void LevelUp()
    {
        if (currentLevel >= 6 || (currentLevel >= GameManager.Instance.CenterLevel && data is not CenterData)) return;
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

    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Skip if the click is over a UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            BuildingInfoPanel panel = FindFirstObjectByType<BuildingInfoPanel>();
            if (panel != null)
            {
                panel.ShowPanel(this);
            }
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
        if (mainCamera == null || canvas == null) return;

        // Get the building's world position
        Vector3 buildingWorldPos = transform.position;

        // Estimate the building's height (using sprite bounds if available)
        float buildingHeight = spriteRenderer != null && spriteRenderer.sprite != null ? spriteRenderer.sprite.bounds.size.y : 1f;

        // Offset the position below the building
        Vector3 offset = new Vector3(0, -buildingHeight/2, 0); // Changed to negative to place below
        Vector3 worldPosBelowBuilding = buildingWorldPos + offset;

        // Convert world position to screen position
        Vector2 screenPos = mainCamera.WorldToScreenPoint(worldPosBelowBuilding);

        // Convert screen position to Canvas position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
            out Vector2 localPoint
        );

        // Adjust for the health bar's pivot (default pivot is center, so offset by half the health bar height)
        Vector2 healthBarSize = healthBarRectTransform.rect.size;
        localPoint.y -= healthBarSize.y * 0.5f; // Move down by half the health bar height to position below

        // Set the health bar's position
        healthBarRectTransform.anchoredPosition = localPoint;
    }

    public int GetLevel() => currentLevel;
}