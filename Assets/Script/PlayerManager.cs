using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float health, maxHealth = 100;
    [SerializeField] private HealthPlayer healthPlayer;
    [SerializeField] private float armor, maxArmor = 10;
    [SerializeField] private ArmorPlayer armorPlayer;
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

    private bool isArmorVisible = false; // Ẩn thanh Armor khi bắt đầu game
    public bool isAlive = true;          // Biến trạng thái isAlive, mặc định là true

    void Awake()
    {
        healthPlayer = GetComponentInChildren<HealthPlayer>();
        armorPlayer = GetComponentInChildren<ArmorPlayer>();
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

        // Ẩn thanh Armor khi bắt đầu game
        armorPlayer.gameObject.SetActive(false);
    }

    void Update()
    {
        // Nếu player đã chết, không thực hiện Update()
        if (!isAlive) return;

        mouseP = m_Camera.ScreenToWorldPoint(Input.mousePosition);
        mouseP.z = transform.position.z;
        Vector3 rotation = mouseP - transform.position;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        rotZ += 160f;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
    }

    void FixedUpdate()
    {
        // Nếu player đã chết, không di chuyển
        if (!isAlive) return;

        rb.linearVelocity = moveInput * speed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!isAlive) return;
        moveInput = context.ReadValue<Vector2>();
        Debug.Log("Move Input: " + moveInput);
    }

    public void TakeDamage(float damage)
    {
        if (!isAlive) return; // Nếu đã chết, không nhận damage

        if (isArmorVisible && armor > 0)
        {
            armor -= damage;
            if (armor <= 0)
            {
                armor = 0;
                armorPlayer.gameObject.SetActive(false);
                isArmorVisible = false;
            }
            armorPlayer.UpdatePlayerArmor(armor, maxArmor);
        }
        else
        {
            health -= damage;
        }

        // ✅ Cập nhật lại thanh máu sau khi trừ
        healthPlayer.UpdatePlayerHealth(health, maxHealth);

        if (health <= 0)
        {
            Die();
        }
    }


    void Die()
    {
        isAlive = false; // Đánh dấu player đã chết
        Time.timeScale = 0;
        gameObject.SetActive(false); // Ẩn đối tượng
        mainmenu.Revive();
    }

    public void Revive()
    {
        // Reset lại các giá trị khi hồi sinh
        health = maxHealth;
        healthPlayer.UpdatePlayerHealth(health, maxHealth);

        armor = maxArmor;
        armorPlayer.UpdatePlayerArmor(armor, maxArmor);

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(true);
        Time.timeScale = 1;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = hammerSprite;
        }
        isAlive = true; // Player được đánh dấu sống lại
        mainmenu.startGame();
        Debug.Log("Player Revived: " + playerName);
    }

    public void ChangeToHammer() { spriteRenderer.sprite = hammerSprite; }
    public void ChangeToSword() { spriteRenderer.sprite = swordSprite; }
    public void ChangeToBow() { spriteRenderer.sprite = bowSprite; }

    public void SetPlayerName(string name)
    {
        playerName.text = name;
        Debug.Log("Player name set to: " + playerName);
    }

    public void FullHeal()
    {
        health = maxHealth;
        healthPlayer.UpdatePlayerHealth(health, maxHealth);
        Debug.Log("Fully healed! Current health: " + health);

        armor = maxArmor;
        armorPlayer.UpdatePlayerArmor(armor, maxArmor);
        Debug.Log("Fully healed! Current armor: " + armor);
    }

    public void ToggleArmor()
    {
        if (!isArmorVisible)
        {
            armor = maxArmor;
            armorPlayer.UpdatePlayerArmor(armor, maxArmor);
            armorPlayer.gameObject.SetActive(true);
            isArmorVisible = true;
        }
    }
}
