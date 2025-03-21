using UnityEngine;
using UnityEngine.EventSystems; // Add this for EventSystem

public abstract class Building : MonoBehaviour
{
    public BuildingData data;
    protected int currentLevel = 1;
    public int currentHealth;
    private SpriteRenderer spriteRenderer; // Cache the SpriteRenderer for efficiency

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite(); // Set the initial sprite for Level 1
        UpdateStats();
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
            Debug.Log($"{data.buildingName} leveled up to {currentLevel}");
            UpdatePanelIfDisplayed(); // Update the panel to reflect the new level
        }
    }

    protected virtual void UpdateStats()
    {
        currentHealth = (int)(data.maxHealth * data.healthMultipliers[currentLevel - 1]);
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
        if (currentHealth <= 0) Destroy(gameObject);
    }

    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Skip if the click is over a UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log($"Click on {data.buildingName} ignored because it's over a UI element.");
                return;
            }

            BuildingInfoPanel panel = FindFirstObjectByType<BuildingInfoPanel>();
            if (panel != null)
            {
                Debug.Log($"Click on {data.buildingName}, showing panel.");
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

    public int GetLevel() => currentLevel;
}