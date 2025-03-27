using TMPro;
using UnityEngine;

public class PlayerCollect : MonoBehaviour
{
    [SerializeField] private float timeDestroy = 0.2f;
    [SerializeField] private int damage = 10;
    private ObjectPool objectPool;
    private float lifeTime;
    private bool hasDealtDamage = false;
    private float spawnTime;

    void OnEnable()
    {
        lifeTime = Time.time + timeDestroy;
        hasDealtDamage = false;
        GetComponent<Collider2D>().enabled = true;
        spawnTime = Time.time;
    }
    private void Awake()
    {

    }
    public void SetPool(ObjectPool pool)
    {
        objectPool = pool;
    }
    void Update()
    {
        if (Time.time - spawnTime >= lifeTime)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasDealtDamage) return;

        Ressources ressources = collision.GetComponent<Ressources>();
        if (ressources != null)
        {
            Debug.Log("Damage resource");
            ressources.TakeDamage();

            switch (collision.tag)
            {
                case "Rock":
                    ResourceManager.Instance.AddStone(damage);
                    break;
                case "Tree":
                    ResourceManager.Instance.AddWood(damage);
                    break;
            }

            hasDealtDamage = true;
            GetComponent<Collider2D>().enabled = false;
            ReturnToPool();
        }

        // Kiểm tra va chạm với enemy có vũ khí
        EnemyWithWeapon enemyWithWeapon = collision.GetComponent<EnemyWithWeapon>();
        if (enemyWithWeapon != null)
        {
            enemyWithWeapon.TakeDamage(damage);
            ReturnToPool();
            return;
        }

        // Kiểm tra va chạm với enemy không có vũ khí
        EnemyNoWeapon enemyNoWeapon = collision.GetComponent<EnemyNoWeapon>();
        if (enemyNoWeapon != null)
        {
            enemyNoWeapon.TakeDamage(damage);
            ReturnToPool();
            return;
        }
    }
    private void ReturnToPool()
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
