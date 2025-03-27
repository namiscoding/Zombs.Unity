using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float health, maxHealth = 10;
    [SerializeField] private HealthPlayer healthPlayer;
    [SerializeField] private float armor, maxArmor = 0;
    [SerializeField] private ArmorPlayer armorPlayer;
    [SerializeField] private Sprite hammerSprite;
    [SerializeField] private Sprite bowSprite;
    [SerializeField] private GameObject changeToSwordPanel;
    [SerializeField] private GameObject changeToBowPanel;
    [SerializeField] private GameObject changeToAxePanel;
    [SerializeField] public float spinAttackDamage = 2f;  // Sát thương khi tấn công xoay vòng
    [SerializeField] private float spinAttackDuration = 0.5f;  // Thời gian xoay vòng
    [SerializeField] private float spinAttackSpeed = 720f;  // Tốc độ xoay (độ/giây)
    [SerializeField] private float spinAttackCooldown = 0.2f;  // Thời gian giữa các lần gây sát thương
    [SerializeField] private GameObject weaponCollider; // Tham chiếu đến WeaponCollider

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Camera m_Camera;
    private Vector3 mouseP;
    private SpriteRenderer spriteRenderer;
    private Mainmenu mainmenu;
    private ShopManager shopManager;
    private AmorManager amorManager;
    private WP_SwordManager swordManager;
    private WP_BowManager bowManager;
    private WP_AxeManager axeManager;

    public TextMeshProUGUI playerName;
    public bool isAlive = true;
    private bool isArmorVisible = false;

    private bool isSpinning = false;
    private float spinTimer = 0f;
    private Quaternion originalRotation;
    private float lastDamageTime = 0f;
    private HashSet<Collider2D> hitEnemiesThisAttack = new HashSet<Collider2D>();
    private Collider2D weaponColliderComponent;

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
        mainmenu = FindFirstObjectByType<Mainmenu>();
        shopManager = FindFirstObjectByType<ShopManager>();
        amorManager = FindFirstObjectByType<AmorManager>();
        swordManager = FindFirstObjectByType<WP_SwordManager>();
        bowManager = FindFirstObjectByType<WP_BowManager>();
        axeManager = FindFirstObjectByType<WP_AxeManager>();
        if (mainmenu == null) Debug.LogError("Mainmenu not found!");
        if (shopManager == null) Debug.LogError("ShopManager not found!");
        if (amorManager == null) Debug.LogError("AmorManager not found!");
        if (swordManager == null) Debug.LogError("WP_SwordManager not found!");
        if (bowManager == null) Debug.LogError("WP_BowManager not found!");
        if (axeManager == null) Debug.LogError("WP_AxeManager not found!");
        gameObject.SetActive(false);
        changeToSwordPanel.SetActive(false);
        changeToBowPanel.SetActive(false);
        changeToAxePanel.SetActive(true);
        SyncWithAmorManager();
        
        // Lấy tham chiếu đến Collider2D của WeaponCollider
        if (weaponCollider != null)
        {
            weaponColliderComponent = weaponCollider.GetComponent<Collider2D>();
            // Đảm bảo collider bị tắt khi không tấn công
            if (weaponColliderComponent != null)
            {
                weaponColliderComponent.enabled = false;
            }
        }
    }

    void Update()
    {
        if (isSpinning)
        {
            // Xử lý tấn công xoay vòng
            HandleSpinAttack();
            
            // Kiểm tra nếu thả chuột phải thì dừng xoay
            if (Input.GetMouseButtonUp(1))
            {
                StopSpinAttack();
            }
        }
        else
        {
            // Xử lý quay theo chuột như bình thường
            mouseP = m_Camera.ScreenToWorldPoint(Input.mousePosition);
            mouseP.z = transform.position.z;
            Vector3 rotation = mouseP - transform.position;
            float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            rotZ += 160f;
            transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
        }

        // Kiểm tra nhấn chuột phải để tấn công xoay vòng
        if (Input.GetMouseButtonDown(1) && !isSpinning)
        {
            StartSpinAttack();
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * speed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void TakeDamage(float damage)
    {
        if (!isAlive) return; // Nếu đã chết, không nhận damage

        // Ghi log khi nhận damage
        Debug.Log($"Player nhận {damage} sát thương!");

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

        // Ghi log trạng thái sau khi trừ damage
        Debug.Log($"Armor: {armor} / {maxArmor}, Health: {health} / {maxHealth}");

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

        if (amorManager != null)
        {
            amorManager.ResetArmor();
            armor = amorManager.GetCurrentArmor();
            maxArmor = amorManager.GetMaxArmor();
        }
        else
        {
            armor = maxArmor;
        }
        armorPlayer.UpdatePlayerArmor(armor, maxArmor);

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(true);
        Time.timeScale = 1;

        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (spriteRenderer != null) spriteRenderer.sprite = hammerSprite;
        if (mainmenu != null) mainmenu.startGame();
    }

    public void ChangeToHammer()
    {
        if (spriteRenderer != null) spriteRenderer.sprite = hammerSprite;
    }

    public void ChangeToSword()
    {
        if (spriteRenderer != null && swordManager != null)
        {
            spriteRenderer.sprite = swordManager.GetCurrentSwordSprite();
            Debug.Log("Changed to sword sprite: " + spriteRenderer.sprite.name);
        }
    }

    public void ChangeToBow()
    {
        if (spriteRenderer != null && bowManager != null)
        {
            spriteRenderer.sprite = bowManager.GetCurrentBowSprite();
            Debug.Log("Changed to bow sprite: " + spriteRenderer.sprite.name);
        }
    }

    public void ChangeToAxe()
    {
        if (spriteRenderer != null && axeManager != null)
        {
            spriteRenderer.sprite = axeManager.GetCurrentAxeSprite();
            Debug.Log("Changed to axe sprite: " + spriteRenderer.sprite.name);
        }
    }

    public void SetPlayerName(string name)
    {
        if (playerName != null) playerName.text = name;
    }

    public void FullHeal()
    {
        health = maxHealth;
        healthPlayer.UpdatePlayerHealth(health, maxHealth);

        if (amorManager != null)
        {
            amorManager.SetCurrentArmor(amorManager.GetMaxArmor());
            armor = amorManager.GetCurrentArmor();
            maxArmor = amorManager.GetMaxArmor();
        }
        else
        {
            armor = maxArmor;
        }
        armorPlayer.UpdatePlayerArmor(armor, maxArmor);
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
        if (amorManager != null)
        {
            armor = amorManager.GetCurrentArmor();
            maxArmor = amorManager.GetMaxArmor();
            if (maxArmor > 0)
            {
                if (armorPlayer != null)
                {
                    armorPlayer.gameObject.SetActive(true);
                    isArmorVisible = true;
                    armorPlayer.UpdatePlayerArmor(armor, maxArmor);
                }
                else
                {
                    Debug.LogError("armorPlayer not assigned in Inspector!");
                }
            }
            else
            {
                if (armorPlayer != null)
                {
                    armorPlayer.gameObject.SetActive(false);
                    isArmorVisible = false;
                }
            }
        }
    }

    public void SyncWithAmorManager()
    {
        if (amorManager != null)
        {
            maxArmor = amorManager.GetMaxArmor();
            armor = amorManager.GetCurrentArmor();
            float previousMaxHealth = maxHealth;
            maxHealth = 10 + amorManager.GetMaxHealthBonus();
            health += maxHealth - previousMaxHealth;
            healthPlayer.UpdatePlayerHealth(health, maxHealth);
            armorPlayer.UpdatePlayerArmor(armor, maxArmor);
            if (maxArmor > 0 && armor > 0)
            {
                isArmorVisible = true;
                armorPlayer.gameObject.SetActive(true);
            }
            else
            {
                isArmorVisible = false;
                armorPlayer.gameObject.SetActive(false);
            }
        }
    }

    public void ToggleSword()
    {
        if (changeToSwordPanel != null && swordManager != null)
        {
            changeToSwordPanel.SetActive(true);
            var swordImage = changeToSwordPanel.GetComponent<UnityEngine.UI.Image>();
            if (swordImage != null)
            {
                swordImage.sprite = swordManager.GetCurrentSwordSpriteWP();
                Debug.Log("Sword UI updated with sprite: " + swordImage.sprite.name);
            }
            else
            {
                Debug.LogWarning("Sword Image component not found on changeToSwordPanel!");
            }
        }
        else
        {
            Debug.LogWarning("changeToSwordPanel or swordManager is not assigned!");
        }
    }

    public void ToggleBow()
    {
        if (changeToBowPanel != null && bowManager != null)
        {
            changeToBowPanel.SetActive(true);
            var bowImage = changeToBowPanel.GetComponent<UnityEngine.UI.Image>();
            if (bowImage != null)
            {
                bowImage.sprite = bowManager.GetCurrentBowSpriteWP();
                Debug.Log("Bow UI updated with sprite: " + bowImage.sprite.name);
            }
            else
            {
                Debug.LogError("Bow Image component not found on changeToBowPanel!");
            }
        }
        else
        {
            Debug.LogError("changeToBowPanel or bowManager is not assigned!");
        }
    }

    public void ToggleAxe()
    {
        if (changeToAxePanel != null && axeManager != null)
        {
            changeToAxePanel.SetActive(true);
            var axeImage = changeToAxePanel.GetComponent<UnityEngine.UI.Image>();
            if (axeImage != null)
            {
                axeImage.sprite = axeManager.GetCurrentAxeSpriteWP();
                Debug.Log("Axe UI updated with sprite: " + axeImage.sprite.name);
            }
            else
            {
                Debug.LogError("Axe Image component not found on changeToAxePanel!");
            }
        }
        else
        {
            Debug.LogError("changeToAxePanel or axeManager is not assigned!");
        }
    }

    private void StartSpinAttack()
    {
        isSpinning = true;
        spinTimer = 0f;
        lastDamageTime = 0f;
        originalRotation = transform.rotation;
        hitEnemiesThisAttack.Clear();
        
        // Bật collider của vũ khí khi bắt đầu tấn công
        if (weaponColliderComponent != null)
        {
            weaponColliderComponent.enabled = true;
        }
    }

    private void StopSpinAttack()
    {
        isSpinning = false;
        
        // Tắt collider của vũ khí khi kết thúc tấn công
        if (weaponColliderComponent != null)
        {
            weaponColliderComponent.enabled = false;
        }
        
        // Khi dừng xoay, cập nhật lại hướng quay theo chuột
        mouseP = m_Camera.ScreenToWorldPoint(Input.mousePosition);
        mouseP.z = transform.position.z;
        Vector3 rotation = mouseP - transform.position;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        rotZ += 160f;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
    }

    private void HandleSpinAttack()
    {
        // Xoay nhân vật liên tục với tốc độ cố định
        transform.Rotate(0, 0, spinAttackSpeed * Time.deltaTime);
        
        // Kiểm tra va chạm với kẻ địch và gây sát thương theo chu kỳ
        if (Time.time >= lastDamageTime + spinAttackCooldown)
        {
            hitEnemiesThisAttack.Clear(); // Reset danh sách enemy đã bị tấn công
            lastDamageTime = Time.time;
            
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, 1.5f);
            foreach (Collider2D enemy in hitEnemies)
            {
                // Kiểm tra va chạm với EnemyWithWeapon
                if (enemy.CompareTag("bodyEnemy") || enemy.CompareTag("Enemy"))
                {
                    EnemyWithWeapon enemyWithWeapon = enemy.GetComponent<EnemyWithWeapon>();
                    if (enemyWithWeapon != null && !hitEnemiesThisAttack.Contains(enemy))
                    {
                        enemyWithWeapon.TakeDamage((int)spinAttackDamage);
                        Debug.Log($"Đã gây {spinAttackDamage} sát thương cho EnemyWithWeapon");
                        hitEnemiesThisAttack.Add(enemy);
                    }
                    
                    // Kiểm tra va chạm với EnemyNoWeapon
                    EnemyNoWeapon enemyNoWeapon = enemy.GetComponent<EnemyNoWeapon>();
                    if (enemyNoWeapon != null && !hitEnemiesThisAttack.Contains(enemy))
                    {
                        enemyNoWeapon.TakeDamage((int)spinAttackDamage);
                        Debug.Log($"Đã gây {spinAttackDamage} sát thương cho EnemyNoWeapon");
                        hitEnemiesThisAttack.Add(enemy);
                    }
                }
            }
        }
    }

}