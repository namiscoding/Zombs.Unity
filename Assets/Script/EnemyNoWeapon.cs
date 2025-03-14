using UnityEngine;
using System.Collections;

public class EnemyNoWeapon : MonoBehaviour
{
    public enum EnemyType { Blue, Green, Red, Yellow }
    public EnemyType enemyType;
    public float speed;
    public int maxHealth;
    private int currentHealth;
    public int attackDamage; // 💥 Sát thương khi tấn công

    public float stopDistance = 0.5f;
    public float attackAmplitude = 15f;
    public float attackFrequency = 5f;
    public float attackDuration = 1f;

    private Transform player;
    private PlayerHealth playerHealth;
    private Rigidbody2D rb;
    private bool isAttacking = false;
    private float initialAngleZ;
    private Vector3 initialPosition;

    public Transform armTransform;
    private Vector3 armLocalPosition;

    void Start()
    {
        SetAttributesBasedOnType();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        rb = GetComponent<Rigidbody2D>() ?? gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (armTransform != null)
            armLocalPosition = armTransform.localPosition;

        currentHealth = maxHealth;
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

        FixArmPosition();
    }

    void RotateTowardsPlayer(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void FixArmPosition()
    {
        if (armTransform != null)
            armTransform.localPosition = armLocalPosition;
    }

    IEnumerator AttackBehavior()
    {
        float elapsed = 0f;
        while (elapsed < attackDuration)
        {
            elapsed += Time.deltaTime;
            float attackAngle = Mathf.Sin(elapsed * attackFrequency * Mathf.PI * 2) * attackAmplitude;
            transform.rotation = Quaternion.Euler(0f, 0f, initialAngleZ + attackAngle);
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0f, 0f, initialAngleZ);
        isAttacking = false;

        // 💥 Sau khi tấn công, gây damage lên Player
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log($"{enemyType} gây {attackDamage} sát thương lên Player!");
        }
    }

    void SetAttributesBasedOnType()
    {
        switch (enemyType)
        {
            case EnemyType.Blue:
                speed = 3f;
                maxHealth = 100;
                attackDamage = 10;
                break;
            case EnemyType.Green:
                speed = 2f;
                maxHealth = 120;
                attackDamage = 15;
                break;
            case EnemyType.Red:
                speed = 6f;
                maxHealth = 80;
                attackDamage = 8;
                break;
            case EnemyType.Yellow:
                speed = 4f;
                maxHealth = 150;
                attackDamage = 12;
                break;
        }
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{enemyType} nhận {damage} sát thương. Máu còn lại: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{enemyType} đã bị tiêu diệt!");
        Destroy(gameObject);
    }
}
