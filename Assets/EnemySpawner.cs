using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyWaveGroup
    {
        public string weaponTypeName; // Eg. Sword, Mace, Shovel
        public GameObject[] enemyPrefabs; // Prefabs with this weapon
    }

    public EnemyWaveGroup[] earlyWaveEnemies; // Wave 1–4 grouped by weapon
    public GameObject[] bossPrefabs; // Boss1, Boss2 prefabs

    public Transform[] spawnPoints; // 4 spawn positions
    public float timeBetweenWaves = 5f;

    private int currentWave = 0;
    private bool isSpawning = false;

    void Start()
    {   
        StartCoroutine(SpawnWaveLoop());
    }

    IEnumerator SpawnWaveLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenWaves);
            currentWave++;
            StartCoroutine(SpawnWave(currentWave));
        }
    }

    IEnumerator SpawnWave(int wave)
    {
        isSpawning = true;
        int totalEnemies = Mathf.Clamp(wave + 2, 4, 20); // gradually increase
        List<GameObject> spawnPool = new List<GameObject>();

        if (wave <= 4)
        {
            // Each wave uses a specific weapon group
            int index = (wave - 1) % earlyWaveEnemies.Length;
            spawnPool.AddRange(earlyWaveEnemies[index].enemyPrefabs);
        }
        else
        {
            // Mix all early wave enemies
            foreach (var group in earlyWaveEnemies)
                spawnPool.AddRange(group.enemyPrefabs);
        }

        // Optionally add a boss
        int bossCount = (wave >= 5 && Random.value < 0.3f) ? 1 : 0; // 30% chance
        if (bossCount > 0 && bossPrefabs.Length > 0)
        {
            GameObject boss = bossPrefabs[Random.Range(0, bossPrefabs.Length)];
            SpawnEnemyAtRandomPoint(boss);
            totalEnemies = Mathf.Max(1, totalEnemies - 1); // limit
            yield return new WaitForSeconds(0.5f);
        }

        for (int i = 0; i < totalEnemies; i++)
        {
            GameObject prefab = spawnPool[Random.Range(0, spawnPool.Count)];
            SpawnEnemyAtRandomPoint(prefab);
            yield return new WaitForSeconds(0.3f);
        }

        isSpawning = false;
    }

    void SpawnEnemyAtRandomPoint(GameObject prefab)
    {
        int pointIndex = Random.Range(0, spawnPoints.Length);
        Instantiate(prefab, spawnPoints[pointIndex].position, Quaternion.identity);
    }
}
