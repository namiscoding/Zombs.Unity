using UnityEngine;
using System.Collections.Generic;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance; // Singleton để truy cập từ các script khác

    [System.Serializable]
    public class EnemyPoolItem
    {
        public Enemy.EnemyType enemyType;
        public GameObject enemyPrefab; // Prefab của kẻ thù
        public int poolSize = 5; // Số lượng đối tượng trong pool
    }

    public List<EnemyPoolItem> enemyPools; // Danh sách các pool cho từng loại kẻ thù
    private Dictionary<Enemy.EnemyType, Queue<Enemy>> pools; // Dictionary lưu trữ các pool
    private Transform player; // Transform của Player để truyền cho kẻ thù

    void Awake()
    {
        // Thiết lập Singleton
        Instance = this;

        // Tìm Player
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player with tag 'Player' not found in the scene!");
            return;
        }

        // Khởi tạo các pool
        pools = new Dictionary<Enemy.EnemyType, Queue<Enemy>>();
        foreach (var poolItem in enemyPools)
        {
            Queue<Enemy> enemyQueue = new Queue<Enemy>();

            // Tạo poolSize đối tượng cho từng loại kẻ thù
            for (int i = 0; i < poolItem.poolSize; i++)
            {
                GameObject obj = Instantiate(poolItem.enemyPrefab);
                obj.SetActive(false); // Tắt đối tượng ban đầu
                Enemy enemy = obj.GetComponent<Enemy>();
                enemy.enemyType = poolItem.enemyType; // Gán loại kẻ thù
                enemyQueue.Enqueue(enemy);
            }

            pools.Add(poolItem.enemyType, enemyQueue);
        }
    }

    // Lấy một kẻ thù từ pool
    public Enemy GetEnemy(Enemy.EnemyType type, Vector2 position)
    {
        if (!pools.ContainsKey(type))
        {
            Debug.LogWarning($"No pool for enemy type {type}!");
            return null;
        }

        Queue<Enemy> queue = pools[type];
        Enemy enemy;

        // Nếu pool rỗng, tạo thêm một đối tượng mới
        if (queue.Count == 0)
        {
            EnemyPoolItem poolItem = enemyPools.Find(item => item.enemyType == type);
            GameObject obj = Instantiate(poolItem.enemyPrefab);
            enemy = obj.GetComponent<Enemy>();
            enemy.enemyType = type;
        }
        else
        {
            enemy = queue.Dequeue();
        }

        // Khởi tạo lại kẻ thù
        enemy.transform.position = position;
        //enemy.Initialize(player);
        return enemy;
    }

    // Trả lại kẻ thù vào pool
    public void ReturnEnemy(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
        pools[enemy.enemyType].Enqueue(enemy);
    }
}