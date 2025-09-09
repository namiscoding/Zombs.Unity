using UnityEngine;

public class PlayerCollect : MonoBehaviour
{
    [SerializeField] private float timeDestroy = 0.2f;
    private WP_AxeManager wpAxeManager;
    private ObjectPool objectPool;
    private float lifeTime;
    private bool hasInteracted = false;
    private WeapomColider weapomColider; // Sử dụng tên WeapomColider

    private void Start()
    {
        wpAxeManager = FindFirstObjectByType<WP_AxeManager>();
        weapomColider = GetComponent<WeapomColider>();
        if (wpAxeManager == null)
        {
            Debug.LogError("PlayerCollect: WP_AxeManager not found in the scene!");
        }
        if (weapomColider == null)
        {
            Debug.LogError("PlayerCollect: WeapomColider not found on this GameObject!");
        }
    }

    private void OnEnable()
    {
        lifeTime = Time.time + timeDestroy;
        hasInteracted = false;
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = true;
        Debug.Log("PlayerCollect: Enabled");
    }

    public void SetPool(ObjectPool pool)
    {
        objectPool = pool;
    }

    private void Update()
    {
        if (Time.time > lifeTime)
        {
            ReturnToPoolOrDestroy();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasInteracted) return;

        Debug.Log("PlayerCollect: Trigger entered with " + collision.gameObject.name + " (Tag: " + collision.tag + ")");

        // Xử lý thu hoạch tài nguyên (chỉ với Axe)
        Ressources ressources = collision.GetComponent<Ressources>();
        if (ressources != null && wpAxeManager != null)
        {
            Debug.Log("PlayerCollect: Harvesting resource");
            ressources.TakeDamage();

            switch (collision.tag)
            {
                case "Rock":
                    ResourceManager.Instance.AddStone(wpAxeManager.GetCurrentCollect());
                    Debug.Log($"PlayerCollect: Collected {wpAxeManager.GetCurrentCollect()} stone");
                    break;
                case "Tree":
                    ResourceManager.Instance.AddWood(wpAxeManager.GetCurrentCollect());
                    Debug.Log($"PlayerCollect: Collected {wpAxeManager.GetCurrentCollect()} wood");
                    break;
            }

            hasInteracted = true;
            GetComponent<Collider2D>().enabled = false;
            ReturnToPoolOrDestroy();
        }

        // Xử lý tấn công kẻ địch (không return để cả hai đều có thể xảy ra nếu cần)
        if (weapomColider != null && (collision.CompareTag("bodyEnemy") || collision.CompareTag("Enemy")))
        {
            weapomColider.OnTriggerEnter2D(collision);
            hasInteracted = true;
            GetComponent<Collider2D>().enabled = false;
            ReturnToPoolOrDestroy();
        }
    }

    private void ReturnToPoolOrDestroy()
    {
        if (objectPool != null)
        {
            objectPool.ReturnObject(gameObject);
            Debug.Log("PlayerCollect: Returned to pool");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("PlayerCollect: Destroyed");
        }
    }
}