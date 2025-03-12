using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public enum EnemyType { Blue, Green, Red, Yellow }
    public EnemyType enemyType;
    public float speed;
    public float stopDistance = 0.5f;
    public float attackAmplitude = 30f; // Biên độ xoay nhỏ để mô phỏng tấn công
    public float attackFrequency = 5f; // Tần số xoay nhanh để mô phỏng đấm
    public float attackDuration = 1f; // Thời gian duy trì tấn công

    private Transform player;
    private Rigidbody2D rb;
    private bool isAttacking = false;
    private Vector3 initialPosition;
    private float initialAngleZ; // Góc ban đầu quanh trục Z
    public Transform armTransform;
    void Start()
    {
        SetSpeedBasedOnType();
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
    }

    void RotateTowardsPlayer(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Xoay cánh tay hướng về người chơi
        if (armTransform != null)
        {
            float armAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            armTransform.rotation = Quaternion.Euler(0f, 0f, armAngle);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isAttacking = true;
            initialPosition = transform.position;
            initialAngleZ = transform.rotation.eulerAngles.z; // Lưu góc ban đầu quanh trục Z
            StartCoroutine(AttackBehavior());
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isAttacking = false; // Dừng tấn công khi rời khỏi Player
        }
    }

    IEnumerator AttackBehavior()
    {
        float elapsed = 0f;
        while (isAttacking && elapsed < attackDuration) // Giới hạn thời gian tấn công
        {
            elapsed += Time.deltaTime;
            // Tính toán góc dao động từ -attackAmplitude đến +attackAmplitude quanh trục Z
            float attackAngle = Mathf.Sin(elapsed * attackFrequency * Mathf.PI * 2) * attackAmplitude;
            transform.rotation = Quaternion.Euler(0f, 0f, initialAngleZ + attackAngle); // Xoay quanh trục Z

            // Giữ vị trí ban đầu (có thể bỏ nếu muốn thêm chuyển động)
            transform.position = initialPosition;

            yield return null;
        }
        // Quay về góc ban đầu khi kết thúc
        transform.rotation = Quaternion.Euler(0f, 0f, initialAngleZ);
        isAttacking = false;
    }

    void SetSpeedBasedOnType()
    {
        switch (enemyType)
        {
            case EnemyType.Blue:
                speed = 3f; // Tăng tốc độ
                break;
            case EnemyType.Green:
                speed = 2f; // Tăng tốc độ
                break;
            case EnemyType.Red:
                speed = 6f; // Tăng tốc độ
                break;
            case EnemyType.Yellow:
                speed = 4f; // Tăng tốc độ
                break;
        }
    }
}