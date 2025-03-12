using UnityEngine;

public class Ressources : MonoBehaviour
{
    [SerializeField] protected float maxHp = 50f;
    public float currentHp;
    [SerializeField] protected int quality;
    protected GameManager gameManager;
    private Rigidbody2D rb;
    private Animator animator;
    private ObjectPool objectPool;
    [SerializeField] private float collectDuration = 0.01f;  // Thời gian duy trì animation isCollect
    private float collectEndTime = 0f;

    void OnEnable()
    {
        objectPool = FindAnyObjectByType<ObjectPool>();
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHp = maxHp;
    }

    void Update()
    {
        // Tắt animation isCollect sau khi hết thời gian
        if (Time.time >= collectEndTime && animator != null && animator.GetBool("isCollect"))
        {
            animator.SetBool("isCollect", false);
        }
    }

    public virtual void TakeDamage(float damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);

        // Bật animation isCollect khi nhận sát thương
        if (animator != null)
        {
            animator.SetBool("isCollect", true);
            collectEndTime = Time.time + collectDuration;  // Cài thời gian tắt animation
        }

        if (currentHp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (animator != null)
        {
            animator.SetBool("isCollect", false);  // Tắt animation khi chết
        }

        ObjectPool pool = transform.root.GetComponent<ObjectPool>();
        if (pool != null)
        {
            pool.ReturnObject(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
