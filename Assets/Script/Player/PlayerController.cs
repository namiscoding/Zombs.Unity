using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    [SerializeField] private float maxHp = 100f;
    private float currentHp;
    [SerializeField] private GameManager gameManager;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        //RotatePlayer();
    }

    //void RotatePlayer()
    //{
    //    if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width ||
    //        Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height)
    //    {
    //        return;
    //    }

    //    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z * -1));
    //    mouseWorldPosition.z = transform.position.z;

    //    Vector3 displacement = mouseWorldPosition - transform.position;
    //    float angle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg;

    //    transform.rotation = Quaternion.Euler(0, 0, angle + 180f);

    //    // Send the angle to Animator
    //    animator.SetFloat("Angle", angle);
    //}

    void MovePlayer()
    {
        Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        rb.linearVelocity = playerInput.normalized * moveSpeed;
    }
}
