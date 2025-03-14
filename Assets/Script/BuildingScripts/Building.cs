using UnityEngine;

public abstract class Building : MonoBehaviour
{
    public BuildingData data;
    protected int currentLevel = 1;
    protected int currentHealth;

    protected virtual void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = data.sprite;
        UpdateStats();
    }

    public virtual void LevelUp()
    {
        if (currentLevel >= 5 || currentLevel >= GameManager.Instance.CenterLevel) return;
        int nextLevelIndex = currentLevel - 1;
        if (ResourceManager.Instance.CanAfford(data.levelUpCosts[nextLevelIndex]))
        {
            ResourceManager.Instance.SpendResources(data.levelUpCosts[nextLevelIndex]);
            currentLevel++;
            UpdateStats();
            Debug.Log($"{data.buildingName} leveled up to {currentLevel}");
        }
    }

    protected virtual void UpdateStats()
    {
        currentHealth = (int)(data.maxHealth * data.healthMultipliers[currentLevel - 1]);
    }

    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) Destroy(gameObject);
    }

    public int GetLevel() => currentLevel;
}