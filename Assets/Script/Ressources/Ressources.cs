using UnityEngine;

public class Ressources : MonoBehaviour
{
    protected GameManager gameManager;
    private Rigidbody2D rb;
    private Animator animator;
    private ObjectPool objectPool;
    [SerializeField] private float collectDuration = 0.01f;
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
        if (animator != null)
        {
            animator.SetBool("isCollect", true);
            collectEndTime = Time.time + collectDuration;
        }
    }
}
