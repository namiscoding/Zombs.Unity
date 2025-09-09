using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState 
{ 
    Idle, 
    AttackingPlayer, 
    AttackingBase,
    AttackingBuilding 
}
public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyWaveGroup
    {
        public string weaponTypeName; // Eg. Sword, Mace, Shovel
        public GameObject[] enemyPrefabs; // Prefabs with this weapon
    }

    public EnemyWaveGroup[] earlyWaveEnemies; // Wave 1-4 grouped by weapon
    public GameObject[] bossPrefabs; // Boss1, Boss2 prefabs

    public float spawnRadius = 10f; // Radius around center to spawn enemies
    public float minSpawnDistance = 5f; // Minimum distance from center to spawn enemies
    public float timeBetweenWaves = 5f;
    public Transform playerTransform; // Reference to player (still useful for some logic)
    
    // Add reference to center
    private Transform centerTransform;
    private TimeManager TimeManager;
    private int currentWave = 0;
    private bool isSpawning = false;
    private Coroutine spawnCoroutine;

    // Method to start spawning, called by GameManager
    public void StartSpawning()
    {
        if (!isSpawning)
        {
            // Find player if not assigned
            if (playerTransform == null)
            {
                playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
                if (playerTransform == null)
                {
                    Debug.LogWarning("Player not found! Some functionality may be limited.");
                }
            }
            
            // Find center
            Center center = FindObjectOfType<Center>();
            if (center != null)
            {
                centerTransform = center.transform;
                Debug.Log("Found Center for enemy spawning!");
            }
            else
            {
                Debug.LogError("Center not found! Enemy spawning will use fallback position.");
                // Use spawner's position as fallback if no center exists
                centerTransform = transform;
            }
            
            currentWave = 0;
            spawnCoroutine = StartCoroutine(SpawnWaveLoop());
            Debug.Log("Enemy spawning started!");
        }
    }

    // Method to stop spawning, called by GameManager
    public void StopSpawning()
    {
        if (isSpawning && spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            isSpawning = false;
            Debug.Log("Enemy spawning stopped!");
        }
    }

    IEnumerator SpawnWaveLoop()
    {
        isSpawning = true;
        while (isSpawning)
        {
            yield return new WaitForSeconds(timeBetweenWaves);
            currentWave++;
            StartCoroutine(SpawnWave(currentWave));
        }
    }

    IEnumerator SpawnWave(int wave)
    {
        Debug.Log($"Spawning Wave {wave}");
        int totalEnemies = Mathf.Clamp(wave + 5, 10, 100); // gradually increase
        List<GameObject> spawnPool = new List<GameObject>();

        // For early waves (1-4), only use one type of enemy per wave
        if (wave <= 4)
        {
            // Each wave uses a specific weapon group
            int index = (wave - 1) % earlyWaveEnemies.Length;
            spawnPool.AddRange(earlyWaveEnemies[index].enemyPrefabs);
            Debug.Log($"Wave {wave}: Using {earlyWaveEnemies[index].weaponTypeName} enemies");
        }
        else
        {
            // Mix all early wave enemies
            foreach (var group in earlyWaveEnemies)
                spawnPool.AddRange(group.enemyPrefabs);
            Debug.Log($"Wave {wave}: Using mixed enemies");
        }

        // Optionally add a boss
        int bossCount = (wave >= 5 && Random.value < 0.3f) ? 1 : 0; // 30% chance
        if (bossCount > 0 && bossPrefabs.Length > 0)
        {
            GameObject boss = bossPrefabs[Random.Range(0, bossPrefabs.Length)];
            SpawnEnemyAroundCenter(boss);
            totalEnemies = Mathf.Max(1, totalEnemies - 1); // limit
            Debug.Log($"Wave {wave}: Spawned a boss!");
            yield return new WaitForSeconds(0.5f);
        }

        for (int i = 0; i < totalEnemies; i++)
        {
            GameObject prefab = spawnPool[Random.Range(0, spawnPool.Count)];
            SpawnEnemyAroundCenter(prefab);
            yield return new WaitForSeconds(0.3f);
        }
    }

    void SpawnEnemyAroundCenter(GameObject prefab)
    {
        if (centerTransform == null) return;
        
        // Get random position around center between min and max distance
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minSpawnDistance, spawnRadius);
        Vector3 spawnPosition = centerTransform.position + new Vector3(randomDirection.x, 0, randomDirection.y) * randomDistance;
        
        // Instantiate enemy at random position
        Instantiate(prefab, spawnPosition, Quaternion.identity);
    }
    
    // Draw spawn radius in editor for visualization
    private void OnDrawGizmosSelected()
    {
        // Determine center for visualization
        Vector3 center;
        if (Application.isPlaying && centerTransform != null)
        {
            center = centerTransform.position;
        }
        else
        {
            // Try to find center in editor for visualization
            Center centerObject = FindObjectOfType<Center>();
            center = centerObject != null ? centerObject.transform.position : transform.position;
        }
        
        // Draw outer spawn radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, spawnRadius);
        
        // Draw inner spawn radius (minimum distance)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, minSpawnDistance);
        
        // Draw small markers around the circle to better visualize spawn points
        int segments = 16;
        Gizmos.color = Color.red;
        for (int i = 0; i < segments; i++)
        {
            float angle = i * (360f / segments) * Mathf.Deg2Rad;
            Vector3 position = center + new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * 
                ((spawnRadius + minSpawnDistance) / 2); // Position markers between min and max radius
            
            Gizmos.DrawSphere(position, 0.3f);
        }
    }
}
