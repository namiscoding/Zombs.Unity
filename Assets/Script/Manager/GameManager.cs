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

}