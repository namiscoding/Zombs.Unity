using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingInfoPanel : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI statsText;
    public Button upgradeButton;
    public Button sellButton;
    public GameObject panel; // References PanelContent
    [HideInInspector]
    public Building currentBuilding;
    public Camera mainCamera; // Assign the actual Camera in the Inspector

    private RectTransform panelRectTransform;
    private Canvas canvas;
    private Building lastPositionedBuilding; // Track the last building for positioning
    private Vector2 lastSetPosition; // Store the last set position for locking
    private bool lockPosition; // Flag to lock the position after setting

    void Start()
    {
        panel.SetActive(false);
        if (upgradeButton != null) upgradeButton.onClick.AddListener(OnUpgradeClicked);
        if (sellButton != null) sellButton.onClick.AddListener(OnSellClicked);

        // Cache components for positioning
        panelRectTransform = panel.GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        // Fallback to Camera.main if mainCamera is not assigned
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogWarning("No main camera assigned or found in the scene!");
            }
        }
    }

    public void ShowPanel(Building building)
    {
        currentBuilding = building;
        panel.SetActive(true);

        // Only reposition if the building has changed
        if (lastPositionedBuilding != building)
        {
            PositionPanelAboveBuilding();
            lastPositionedBuilding = building;
            lockPosition = true; // Lock the position after setting
        }

        UpdatePanel();
    }

    public void HidePanel()
    {
        panel.SetActive(false);
        currentBuilding = null;
        lastPositionedBuilding = null; // Reset to allow repositioning when shown again
        lockPosition = false; // Unlock position when hiding
    }

    private void PositionPanelAboveBuilding()
    {
        if (currentBuilding == null || panelRectTransform == null || canvas == null || mainCamera == null) return;

        // Get the building's world position
        Vector3 buildingWorldPos = currentBuilding.transform.position;

        // Estimate the building's height (using sprite bounds if available)
        SpriteRenderer spriteRenderer = currentBuilding.GetComponent<SpriteRenderer>();
        float buildingHeight = spriteRenderer != null && spriteRenderer.sprite != null ? spriteRenderer.sprite.bounds.size.y : 1f;

        // Offset the position above the building
        Vector3 offset = new Vector3(0, buildingHeight, 0);
        Vector3 worldPosAboveBuilding = buildingWorldPos + offset;

        // Convert world position to screen position
        Vector2 screenPos = mainCamera.WorldToScreenPoint(worldPosAboveBuilding);

        // Convert screen position to Canvas position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
            out Vector2 localPoint
        );

        // Adjust for the panel's pivot (default pivot is center, so offset by half the panel height)
        Vector2 panelSize = panelRectTransform.rect.size;
        localPoint.y += panelSize.y * 0.5f; // Move up by half the panel height to position above

        // Clamp the position to keep the panel within the screen bounds
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 canvasSize = canvasRect.rect.size;
        Vector2 panelHalfSize = panelSize * 0.5f;
        localPoint.x = Mathf.Clamp(localPoint.x, -canvasSize.x * 0.5f + panelHalfSize.x, canvasSize.x * 0.5f - panelHalfSize.x);
        localPoint.y = Mathf.Clamp(localPoint.y, -canvasSize.y * 0.5f + panelHalfSize.y, canvasSize.y * 0.5f - panelHalfSize.y);

        // Set the panel's position
        panelRectTransform.anchoredPosition = localPoint;
        lastSetPosition = localPoint; // Store the last set position for locking
    }

    void LateUpdate()
    {
        // Lock the position to prevent unexpected changes
        if (lockPosition && panel.activeSelf && panelRectTransform.anchoredPosition != lastSetPosition)
        {
            panelRectTransform.anchoredPosition = lastSetPosition;
        }
    }

    public void UpdatePanel()
    {
        if (currentBuilding == null || currentBuilding.data == null) return;

        string buildingName = currentBuilding.data.buildingName;
        titleText.text = $"{buildingName} - Level {currentBuilding.GetLevel()}";

        // Build two-column stats text
        string stats = "";
        int nextLevel = currentBuilding.GetLevel() + 1;
        bool canUpgrade = nextLevel <= 6; // Updated to 6 levels

        // Health (all buildings)
        float currentHealth = currentBuilding.currentHealth;
        float upgradedHealth = canUpgrade ? currentBuilding.data.maxHealth * currentBuilding.data.healthMultipliers[nextLevel - 1] : currentHealth;
        stats += $"Health: {currentHealth}";
        if (canUpgrade) stats += $" -> {upgradedHealth}\n";
        else stats += "\n";

        // Type-specific stats
        if (currentBuilding.data is TowerData towerData)
        {
            float currentDamage = GetStatValue(towerData.damage, towerData.damageMultipliers);
            float upgradedDamage = canUpgrade ? towerData.damage * towerData.damageMultipliers[nextLevel - 1] : currentDamage;
            stats += $"Damage: {currentDamage}";
            if (canUpgrade) stats += $" -> {upgradedDamage}\n";
            else stats += "\n";

            float currentRange = GetStatValue(towerData.attackRange, towerData.rangeMultipliers);
            float upgradedRange = canUpgrade ? towerData.attackRange * towerData.rangeMultipliers[nextLevel - 1] : currentRange;
            stats += $"Range: {currentRange}";
            if (canUpgrade) stats += $" -> {upgradedRange}\n";
            else stats += "\n";
        }
        else if (currentBuilding.data is CenterData centerData)
        {
            float currentRange = GetStatValue(centerData.baseRange, centerData.rangeMultipliers);
            float upgradedRange = canUpgrade ? centerData.baseRange * centerData.rangeMultipliers[nextLevel - 1] : currentRange;
            stats += $"Range: {currentRange}";
            if (canUpgrade) stats += $" -> {upgradedRange}\n";
            else stats += "\n";
        }
        else if (currentBuilding.data is GoldMinerData goldMinerData)
        {
            float currentGoldRate = GetStatValue(goldMinerData.goldRate, goldMinerData.goldRateMultipliers);
            float upgradedGoldRate = canUpgrade ? goldMinerData.goldRate * goldMinerData.goldRateMultipliers[nextLevel - 1] : currentGoldRate;
            stats += $"Gold Rate: {currentGoldRate}";
            if (canUpgrade) stats += $" -> {upgradedGoldRate}\n";
            else stats += "\n";
        }
        else if (currentBuilding.data is HarvesterData harvesterData)
        {
            float currentHarvestRate = GetStatValue(harvesterData.harvestRate, harvesterData.harvestRateMultipliers);
            float upgradedHarvestRate = canUpgrade ? harvesterData.harvestRate * harvesterData.harvestRateMultipliers[nextLevel - 1] : currentHarvestRate;
            stats += $"Harvest Rate: {currentHarvestRate}";
            if (canUpgrade) stats += $" -> {upgradedHarvestRate}\n";
            else stats += "\n";
        }

        statsText.text = stats;

        // Upgrade and Sell buttons
        if (nextLevel <= 6) // Updated to 6 levels
        {
            BuildingData.ResourceCost upgradeCost = currentBuilding.data.levelUpCosts[nextLevel - 2];
            string upgradeCostText = $"Upgrade ({upgradeCost.wood} wood, {upgradeCost.stone} stone, {upgradeCost.gold} gold)";
            upgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = upgradeCostText;

            bool canAfford = ResourceManager.Instance.CanAfford(upgradeCost) && (currentBuilding.data is CenterData || nextLevel <= GameManager.Instance.CenterLevel);
            upgradeButton.interactable = canAfford;
        }
        else
        {
            upgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Max Level";
            upgradeButton.interactable = false;
        }

        // Handle the Sell button
        BuildingData.ResourceCost sellCost = new BuildingData.ResourceCost { wood = 2, stone = 2, gold = 0 };
        bool isCenter = currentBuilding.data is CenterData;
        if (isCenter)
        {
            sellButton.GetComponentInChildren<TextMeshProUGUI>().text = "Cannot Sell";
            sellButton.interactable = false; // Disable the Sell button for Center
        }
        else
        {
            sellButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Sell ({sellCost.wood} wood, {sellCost.stone} stone)";
            sellButton.interactable = true; // Enable the Sell button for other buildings
        }
    }

    private float GetStatValue(float baseValue, float[] multipliers)
    {
        if (multipliers == null || multipliers.Length < currentBuilding.GetLevel()) return baseValue;
        return baseValue * multipliers[currentBuilding.GetLevel() - 1];
    }

    private void OnUpgradeClicked()
    {
        if (currentBuilding != null)
        {
            currentBuilding.LevelUp();
            UpdatePanel();
            // Do not reposition after upgrade to keep the panel static
        }
    }

    private void OnSellClicked()
    {
        if (currentBuilding != null)
        {
            BuildingData.ResourceCost sellCost = new BuildingData.ResourceCost { wood = 2, stone = 2, gold = 0 };
            ResourceManager.Instance.AddResources(sellCost.wood, sellCost.stone, sellCost.gold);

            // Notify BuildingManager to decrement the count
            BuildingManager buildingManager = FindObjectOfType<BuildingManager>();
            if (buildingManager != null)
            {
                buildingManager.OnBuildingSold(currentBuilding);
            }

            Destroy(currentBuilding.gameObject);
            HidePanel();
        }
    }
}