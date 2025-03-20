using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyWithWeapon : MonoBehaviour
{
    public enum EnemyType { Blue, Green, Red, Yellow, Boss1, Boss2 }
    public EnemyType enemyType;
    public float speed;
    public int maxHealth;
    private int currentHealth;

    public float stopDistance = 1.5f;
    public float attackAmplitude = 30f;
    public float attackFrequency = 1f;
    public float attackDuration = 1f;

    private Transform player;
    private Rigidbody2D rb;
    private bool isAttacking = false;
    private float initialAngleZ;
    private Vector3 initialPosition;

    public Transform armTransform;
    public Transform weaponTransform;
    private Vector3 armLocalPosition;
    private Vector3 weaponLocalPosition;
    private WeaponDamage weaponScript; // 💥 Lưu vũ khí để gây damage
                                       // 💥 Thanh máu
                                       // 💥 Thanh máu
    public Slider healthBar;
    private Image healthBarFill;
    void Start()
    {
        SetAttributesBasedOnType();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>() ?? gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Lấy script WeaponDamage từ weaponTransform
        if (weaponTransform != null)
        {
            weaponScript = weaponTransform.GetComponent<WeaponDamage>();
            weaponLocalPosition = weaponTransform.localPosition;
            weaponLocalPosition += new Vector3(0, 0.1f, 0);
        }

        if (armTransform != null)
            armLocalPosition = armTransform.localPosition;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
            healthBarFill = healthBar.fillRect.GetComponent<Image>(); // Lấy `Image` của Fill
            healthBarFill.color = Color.green; // 💚 Ban đầu màu xanh lá
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 direction = (player.position - transform.position).normalized;

        RotateTowardsPlayer(direction);

        if (distanceToPlayer > stopDistance && !isAttacking)
        {
            rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
        }
        else if (!isAttacking)
        {
            isAttacking = true;
            initialPosition = transform.position;
            initialAngleZ = transform.rotation.eulerAngles.z;

            StartCoroutine(AttackBehavior());
        }

        FixArmAndWeaponPosition();
    }

    void RotateTowardsPlayer(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void FixArmAndWeaponPosition()
    {
        if (armTransform != null)
            armTransform.localPosition = armLocalPosition;

        if (weaponTransform != null)
            weaponTransform.localPosition = weaponLocalPosition;
    }

    IEnumerator AttackBehavior()
    {
        float elapsed = 0f;
        while (elapsed < attackDuration)
        {
            elapsed += Time.deltaTime;

            // Hiệu ứng tấn công
            float attackAngle = Mathf.Sin(elapsed * attackFrequency * Mathf.PI * 2) * attackAmplitude;
            transform.rotation = Quaternion.Euler(0f, 0f, initialAngleZ + attackAngle);

            yield return null;
        }

        // 💥 Sau khi vũ khí chạm, gây damage cho player
        if (weaponScript != null && player != null)
        {
            weaponScript.DealDamage(player.gameObject);
        }

        transform.rotation = Quaternion.Euler(0f, 0f, initialAngleZ);
        isAttacking = false;
    }

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

        // 💥 Cập nhật thanh máu
        if (healthBar != null)
        {
            healthBar.value = currentHealth;

            // 🔥 Thay đổi màu dựa vào phần trăm máu
            if (healthBarFill != null)
            {
                if (currentHealth > maxHealth * 0.6f)
                {
                    healthBarFill.color = Color.green; // 💚 Xanh lá (Máu > 60%)
                }
                else if (currentHealth > maxHealth * 0.3f)
                {
                    healthBarFill.color = Color.yellow; // 💛 Vàng (Máu 30-60%)
                }
                else
                {
                    healthBarFill.color = Color.red; // ❤️ Đỏ (Máu < 30%)
                }
            }

            // Ẩn thanh máu nếu máu = 0
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
}
