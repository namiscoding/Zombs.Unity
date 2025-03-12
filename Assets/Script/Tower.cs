using System.Resources;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public TowerData data;
    private int currentLevel = 1; // Starts at Level 1
    private int currentHealth;

    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = data.sprite;
        UpdateStats();
    }

    public void LevelUp()
    {
        if (currentLevel >= 5) return; // Max level reached

        int nextLevelIndex = currentLevel - 1; // 0-based index for levelUpCosts
        if (ResourceManager.Instance.CanAfford(data.levelUpCosts[nextLevelIndex]))
        {
            ResourceManager.Instance.SpendResources(data.levelUpCosts[nextLevelIndex]);
            currentLevel++;
            UpdateStats();
            Debug.Log($"{data.towerName} leveled up to {currentLevel}");
        }
    }

    private void UpdateStats()
    {
        currentHealth = (int)(data.maxHealth * data.healthMultipliers[currentLevel - 1]);
        // Later: Apply damage, range to attack logic
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) Destroy(gameObject);
    }

    public int GetLevel() => currentLevel;
}