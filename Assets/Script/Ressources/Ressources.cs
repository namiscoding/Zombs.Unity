using UnityEngine;

public class Ressources : MonoBehaviour
{
    protected GameManager gameManager;
    private Rigidbody2D rb;
    protected Animator animator;
    protected ObjectPool objectPool;
    [SerializeField] protected float collectDuration = 0.1f;
    protected float collectEndTime = 0f;

    void OnEnable()
    {
        objectPool = FindAnyObjectByType<ObjectPool>();
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Time.time >= collectEndTime && animator != null && animator.GetBool("isCollect"))
        {
            animator.SetBool("isCollect", false);
        }
    }

    public virtual void TakeDamage()
    {
        //if (animator != null)
        //{
        //    animator.SetBool("isCollect", true);
        //    collectEndTime = Time.time + collectDuration;
        //}
    }
}
