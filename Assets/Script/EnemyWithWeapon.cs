using UnityEngine;
using UnityEngine.UI;

public class EnemyWithWeapon : MonoBehaviour
{
    public enum EnemyType { Blue, Green, Red, Yellow, Boss1, Boss2 }
    public EnemyType enemyType;
    public enum WeaponType { Sword, Mace, Shovel, NoWeapon, Boss1, Boss2 }
    public WeaponType weaponType;
    public float speed;
    public int maxHealth;
    private int currentHealth;
    public int baseDamage;
    public float stopDistance = 2f;
    public float attackCooldown = 1f; // Thời gian cooldown giữa các lần gây sát thương bằng vũ khí (giây)
    public float attackRange = 2f; // Khoảng cách để weapon tấn công
    public float weaponRotationSpeed = 5f; // Tốc độ xoay của weapon
    private Animator animator;
    private Transform player;
    private Rigidbody2D rb;
    private Transform weapon;
    private PlayerManager playerManager;
    public Slider healthBar;
    private Image healthBarFill;
    private float lastAttackTime; // Thời gian lần cuối gây sát thương bằng vũ khí
    private bool isAttacking = false;
    private float initialAngleZ;
    private Vector3 initialPosition;

    public Transform armTransform;
    private Vector3 armLocalPosition;

    void Start()
    {
        animator = GetComponent<Animator>();
        SetDamageByWeaponType();
        SetAttributesBasedOnType();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            playerManager = player.GetComponent<PlayerManager>();
        }

        rb = GetComponent<Rigidbody2D>() ?? gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (armTransform != null)
            armLocalPosition = armTransform.localPosition;

        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
            healthBarFill = healthBar.fillRect.GetComponent<Image>(); // Get `Image` of Fill
            healthBarFill.color = Color.green; // Initial color
        }
    }
    void SetDamageByWeaponType()
    {
        switch (weaponType)
        {
            case WeaponType.Sword: baseDamage = 7; break;
            case WeaponType.Mace: baseDamage = 8; break;
            case WeaponType.Shovel: baseDamage = 9; break;
            case WeaponType.NoWeapon: baseDamage = 10; break;
            case WeaponType.Boss1: baseDamage = 11; break;
            case WeaponType.Boss2: baseDamage = 12; break;
            default: baseDamage = 10; break;
        }
    }



    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 direction = (player.position - transform.position).normalized;

        RotateTowardsPlayer(direction);

        // Nếu player chạy xa ra thì cho phép enemy đuổi tiếp
        if (distanceToPlayer > stopDistance + 0.5f)
        {
            isAttacking = false;
            animator.SetBool("isNearPlayer", false);
        }

        if (distanceToPlayer > stopDistance && !isAttacking)
        {
            rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
        }
        else if (distanceToPlayer <= stopDistance && !isAttacking)
        {
            isAttacking = true;
            initialPosition = transform.position;
            initialAngleZ = transform.rotation.eulerAngles.z;
            animator.SetBool("isNearPlayer", true);
        }
    }



    // Xoay vũ khí về hướng Player
    void RotateTowardsPlayer(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    // Xoay MaceYellow theo hướng của WeaponTransform


    void SetAttributesBasedOnType()
    {
        switch (enemyType)
        {
            case EnemyType.Blue: speed = 3f; maxHealth = 100; break;
            case EnemyType.Green: speed = 2f; maxHealth = 120; break;
            case EnemyType.Red: speed = 6f; maxHealth = 80; break;
            case EnemyType.Yellow: speed = 4f; maxHealth = 150; break;
            case EnemyType.Boss1: speed = 3f; maxHealth = 300; break;
            case EnemyType.Boss2: speed = 5f; maxHealth = 400; break;
        }
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{enemyType} nhận {damage} sát thương. Máu còn lại: {currentHealth}");

        if (healthBar != null)
        {
            healthBar.value = currentHealth;

            if (healthBarFill != null)
            {
                if (currentHealth > maxHealth * 0.6f)
                {
                    healthBarFill.color = Color.green;
                }
                else if (currentHealth > maxHealth * 0.3f)
                {
                    healthBarFill.color = Color.yellow;
                }
                else
                {
                    healthBarFill.color = Color.red;
                }
            }

            if (currentHealth <= 0)
            {
                healthBar.gameObject.SetActive(false);
                Die();
            }
        }
    }

    void Die()
    {
        Debug.Log($"{enemyType} đã bị tiêu diệt!");
        Destroy(gameObject);
    }

    public void OnWeaponHitPlayer(Collider2D other)
    {
        if (playerManager != null && playerManager.isAlive && Time.time - lastAttackTime >= attackCooldown)
        {
            playerManager.TakeDamage(baseDamage);
            Debug.Log($"{enemyType}'s weapon gây {baseDamage} sát thương cho Player!");
            lastAttackTime = Time.time;
        }
    }
}
