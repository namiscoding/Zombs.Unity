using UnityEngine;
using System.Collections;

public class EnemyNoWeapon : MonoBehaviour
{
    public enum EnemyType { Blue, Green, Red, Yellow }
    public EnemyType enemyType;
    public float speed;

    public float stopDistance = 0.5f;

    // Các thông số tấn công (cố định cho mọi loại Enemy)
    private const float attackAmplitude = 15f;
    private const float attackFrequency = 5f;
    private const float attackDuration = 1f;

    private Transform player;
    private Rigidbody2D rb;
    private bool isAttacking = false;
    private Vector3 initialPosition;
    private float initialAngleZ;
    public Transform armTransform; // Cánh tay của Enemy

    void Start()
    {
        SetSpeedBasedOnType(); // Chỉ đặt tốc độ di chuyển, tốc độ đánh giữ nguyên
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>() ?? gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }
    void Update()
    {
        if (player != null && !isAttacking)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            Vector2 direction = (player.position - transform.position).normalized;
            RotateTowardsPlayer(direction);

            if (distanceToPlayer > stopDistance)
            {
                rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                if (!isAttacking)
                {
                    isAttacking = true;
                    initialPosition = transform.position;
                    initialAngleZ = transform.rotation.eulerAngles.z;
                    StartCoroutine(AttackBehavior());
                }
            }
        }

        // Luôn giữ cánh tay hướng về Player
        if (armTransform != null && player != null)
        {
            Vector2 armDirection = (player.position - armTransform.position).normalized;
            float armAngle = Mathf.Atan2(armDirection.y, armDirection.x) * Mathf.Rad2Deg;

            // Đặt lại góc quay của cánh tay
            armTransform.rotation = Quaternion.Euler(0f, 0f, armAngle);
        }
    }

    void RotateTowardsPlayer(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void RotateArmTowardsPlayer()
    {
        if (armTransform == null || player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Giữ góc cánh tay độc lập với góc cơ thể
        armTransform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isAttacking = true;
            initialPosition = transform.position;
            initialAngleZ = transform.rotation.eulerAngles.z;
            StartCoroutine(AttackBehavior());
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isAttacking = false;
        }
    }

    IEnumerator AttackBehavior()
    {
        float elapsed = 0f;
        while (isAttacking && elapsed < attackDuration)
        {
            elapsed += Time.deltaTime;
            float attackAngle = Mathf.Sin(elapsed * attackFrequency * Mathf.PI * 2) * attackAmplitude;
            transform.rotation = Quaternion.Euler(0f, 0f, initialAngleZ + attackAngle);
            transform.position = initialPosition;
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0f, 0f, initialAngleZ);
        isAttacking = false;
    }

    void SetSpeedBasedOnType()
    {
        switch (enemyType)
        {
            case EnemyType.Blue:
                speed = 3f;
                break;
            case EnemyType.Green:
                speed = 2f;
                break;
            case EnemyType.Red:
                speed = 6f;
                break;
            case EnemyType Yellow:
                speed = 4f;
                break;
        }
    }
}
