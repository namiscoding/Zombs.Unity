using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float health, maxHealth = 10;
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
    private ShopManager shopManager;
    public TextMeshProUGUI playerName;

    public int gold;

    private bool isArmorVisible = false;

    void Awake()
    {
        healthPlayer = GetComponentInChildren<HealthPlayer>();
        armorPlayer = GetComponentInChildren<ArmorPlayer>();
    }

    void Start()
    {
        gold = 10000;
        m_Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        if (m_Camera == null) Debug.LogError("Main Camera not found!");
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) Debug.LogError("Rigidbody2D is missing!");
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = hammerSprite;
        mainmenu = FindAnyObjectByType<Mainmenu>();
        shopManager = FindAnyObjectByType<ShopManager>();
        if (mainmenu == null) Debug.LogError("Mainmenu not found!");
        if (shopManager == null)
        {
            Debug.LogWarning("ShopManager not found! Please add a ShopManager component to a GameObject in the scene.");
        }
        gameObject.SetActive(false);

        armorPlayer.gameObject.SetActive(false);
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
        Debug.Log("Move Input: " + moveInput);
    }

    public void TakeDamage(float damage)
    {
        if (isArmorVisible && armor > 0)
        {
            armor -= damage;
            armorPlayer.UpdatePlayerArmor(armor, maxArmor);

            if (armor <= 0)
            {
                armor = 0;
                armorPlayer.UpdatePlayerArmor(armor, maxArmor);
                armorPlayer.gameObject.SetActive(false);
                isArmorVisible = false;
            }
        }
        else
        {
            health -= damage;
            healthPlayer.UpdatePlayerHealth(health, maxHealth);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Time.timeScale = 0;
        gameObject.SetActive(false);
        if (mainmenu != null) mainmenu.Revive();
    }

    public void Revive()
    {
        health = maxHealth;
        healthPlayer.UpdatePlayerHealth(health, maxHealth);

        armor = maxArmor;
        armorPlayer.UpdatePlayerArmor(armor, maxArmor);

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(true);
        Time.timeScale = 1;

        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (spriteRenderer != null) spriteRenderer.sprite = hammerSprite;
        if (mainmenu != null) mainmenu.startGame();
        Debug.Log("Player Revived: " + playerName?.text);
    }

    public void ChangeToHammer() { if (spriteRenderer != null) spriteRenderer.sprite = hammerSprite; }
    public void ChangeToSword() { if (spriteRenderer != null) spriteRenderer.sprite = swordSprite; }
    public void ChangeToBow() { if (spriteRenderer != null) spriteRenderer.sprite = bowSprite; }

    public void SetPlayerName(string name)
    {
        if (playerName != null) playerName.text = name;
        Debug.Log("Player name set to: " + playerName?.text);
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

    public void ToggleUtility()
    {
        if (mainmenu == null)
        {
            Debug.LogError("Mainmenu not found!");
            return;
        }

        if (!mainmenu.Utility.activeSelf)
        {
            mainmenu.BuyUtility();
            if (shopManager != null)
            {
                shopManager.PurchaseHealthPotion();
            }
            else
            {
                Debug.LogWarning("ShopManager not found! Cannot purchase health potion.");
            }
        }
        else
        {
            Debug.Log("Utility is already active, no need to buy again.");
        }
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