using TMPro;
using UnityEngine;

public class PlayerCollect : MonoBehaviour
{
    [SerializeField] private float timeDestroy = 0.2f;
    [SerializeField] private int damage = 10;
    private ObjectPool objectPool;
    private float lifeTime;
    private bool hasDealtDamage = false;
    protected RessourceManager ressourceManager;
    void OnEnable()
    {
        lifeTime = Time.time + timeDestroy;
        hasDealtDamage = false;
        GetComponent<Collider2D>().enabled = true;
    }
    private void Awake()
    {
        ressourceManager = FindFirstObjectByType<RessourceManager>();
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
        if (hasDealtDamage) return;

        Ressources ressources = collision.GetComponent<Ressources>();
        if (ressources != null)
        {
            Debug.Log("Damage resource");
            ressources.TakeDamage();

            switch (collision.tag)
            {
                case "Rock":
                    ressourceManager.AddStone(damage);
                    break;
                case "Tree":
                    ressourceManager.AddWood(damage);
                    break;
            }

            hasDealtDamage = true;
            GetComponent<Collider2D>().enabled = false;
            ReturnToPoolOrDestroy();
        }
    }
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
