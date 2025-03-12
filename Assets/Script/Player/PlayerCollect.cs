using UnityEngine;

public class PlayerCollect : MonoBehaviour
{
    [SerializeField] private float timeDestroy = 0.2f;
    [SerializeField] private int damage = 10;
    private ObjectPool objectPool;
    private float lifeTime;
    private bool hasDealtDamage = false;  // Kiểm tra đã gây sát thương chưa

    void OnEnable()
    {
        lifeTime = Time.time + timeDestroy;
        hasDealtDamage = false;
        GetComponent<Collider2D>().enabled = true;
    }

    public void SetPool(ObjectPool pool)
    {
        objectPool = pool;
    }

    void Update()
    {
        if (Time.time > lifeTime)
        {
            ReturnToPoolOrDestroy();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nếu đã gây sát thương rồi thì không gây lại
        if (hasDealtDamage) return;

        if (collision.CompareTag("Rock") || collision.CompareTag("Tree"))
        {
            Ressources ressources = collision.GetComponent<Ressources>();
            if (ressources != null)
            {
                Debug.Log("Dame ressource");
                ressources.TakeDamage(damage);
                hasDealtDamage = true;  // Đánh dấu đã gây sát thương

                // Tắt Collider ngay sau khi gây sát thương để tránh va chạm nhiều lần
                GetComponent<Collider2D>().enabled = false;

                // Trả về ObjectPool hoặc hủy sau khi gây sát thương
                ReturnToPoolOrDestroy();
            }
        }
    }

    // Trả về pool hoặc hủy nếu không có pool
    private void ReturnToPoolOrDestroy()
    {
        if (objectPool != null)
        {
            objectPool.ReturnObject(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
