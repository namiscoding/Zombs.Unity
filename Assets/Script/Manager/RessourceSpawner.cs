using UnityEngine;

public class RessourceSpawner : MonoBehaviour
{
    [SerializeField] private ObjectPool TreePool;
    [SerializeField] private ObjectPool RockPool;
    [SerializeField] private int numberOfTrees = 10; // Number of trees to spawn
    [SerializeField] private int numberOfRocks = 5;  // Number of rocks to spawn
    [SerializeField] private Vector2 spawnAreaMin;  // Bottom-left corner of spawn area
    [SerializeField] private Vector2 spawnAreaMax;  // Top-right corner of spawn area

    void Start()
    {
        SpawnResources();
    }

    void SpawnResources()
    {
        for (int i = 0; i < numberOfTrees; i++)
        {
            GameObject tree = TreePool.GetObject();
            Vector2 randomPosition = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );
            tree.transform.position = randomPosition;
        }

        for (int i = 0; i < numberOfRocks; i++)
        {
            GameObject rock = RockPool.GetObject();
            Vector2 randomPosition = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );
            rock.transform.position = randomPosition;
        }
    }

    Vector3 GetRandomPosition()
    {
        float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float z = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        return new Vector3(x, 0f, z);
    }

}
