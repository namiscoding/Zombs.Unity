using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float health, maxHealth = 10;
    [SerializeField] private HealthPlayer healthPlayer;
    [SerializeField] private Sprite hammerSprite;
    [SerializeField] private Sprite swordSprite;
    [SerializeField] private Sprite bowSprite;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Camera m_Camera;
    private Vector3 mouseP;
    private SpriteRenderer spriteRenderer;
    private Mainmenu mainmenu;
    public TextMeshProUGUI playerName; 

    void Awake()
    {
        healthPlayer = GetComponentInChildren<HealthPlayer>();
    }

    void Start()
    {
        m_Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        if (m_Camera == null) Debug.LogError("Main Camera not found!");
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) Debug.LogError("Rigidbody2D is missing!");
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = hammerSprite;
        mainmenu = FindAnyObjectByType<Mainmenu>();
        gameObject.SetActive(false);
    }

    void Update()
    {
        mouseP = m_Camera.ScreenToWorldPoint(Input.mousePosition);
        mouseP.z = transform.position.z;
        Vector3 rotation = mouseP - transform.position;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        rotZ += 160f;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * speed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        Debug.Log("Move Input: " + moveInput); // Debug
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Player health: " + health + " (" + playerName + ")");
        healthPlayer.UpdatePlayerHealth(health, maxHealth);
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Time.timeScale = 0;
        gameObject.SetActive(false); // Hide instead of destroy
        mainmenu.Revive();
    }

    public void Revive()
    {
        // Reset health to maximum
        health = maxHealth;
        healthPlayer.UpdatePlayerHealth(health, maxHealth); // Update UI

        // Reset position to starting point
        transform.position = Vector3.zero;

        // Reset rotation
        transform.rotation = Quaternion.identity;

        // Ensure the GameObject is active
        gameObject.SetActive(true);

        // Resume game time
        Time.timeScale = 1;

        // Reset velocity
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Reset to default sprite (hammer)
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = hammerSprite;
        }
        mainmenu.startGame(); // This might be a mistake - consider removing or adjusting logic
        Debug.Log("Player Revived: " + playerName);
    }

    public void ChangeToHammer() { spriteRenderer.sprite = hammerSprite; }
    public void ChangeToSword() { spriteRenderer.sprite = swordSprite; }
    public void ChangeToBow() { spriteRenderer.sprite = bowSprite; }

    // New method to set the player name
    public void SetPlayerName(string name)
    {
        playerName.text = name;
        Debug.Log("Player name set to: " + playerName);
        // You could also update a UI element here if you have a name display
    }
}