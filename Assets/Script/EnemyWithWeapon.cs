using UnityEngine;
using System.Collections;

public class EnemyWithWeapon : MonoBehaviour
{
    public enum EnemyType { Blue, Green, Red, Yellow,Boss1 ,Boss2 }
    public EnemyType enemyType;
    public float speed;
    public float stopDistance = 1.5f;
    public float attackAmplitude = 30f; // Biên độ xoay thân khi tấn công
    public float attackFrequency = 1f; // Tốc độ xoay thân khi tấn công
    public float attackDuration = 1f; // Thời gian tấn công

    private Transform player;
    private Rigidbody2D rb;
    private bool isAttacking = false;
    private float initialAngleZ;
    private Vector3 initialPosition;

    public Transform armTransform;  // Cánh tay
    public Transform weaponTransform; // Vũ khí
    private Vector3 armLocalPosition; // Vị trí cố định của cánh tay
    private Vector3 weaponLocalPosition; // Vị trí cố định của vũ khí

    void Start()
    {
        SetSpeedBasedOnType();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>() ?? gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Lưu vị trí gốc của cánh tay và vũ khí (đảm bảo không bị lệch khi di chuyển)
        if (armTransform != null)
            armLocalPosition = armTransform.localPosition;
        if (weaponTransform != null)
        {
            weaponLocalPosition = weaponTransform.localPosition;
            weaponLocalPosition += new Vector3(0, 0.1f, 0); // Điều chỉnh vị trí
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 direction = (player.position - transform.position).normalized;

        // Xoay thân về phía Player
        RotateTowardsPlayer(direction);

        if (distanceToPlayer > stopDistance && !isAttacking)
        {
            rb.MovePosition(rb.position + direction * speed * Time.deltaTime); // Di chuyển đến Player
        }
        else if (!isAttacking)
        {
            isAttacking = true;
            initialPosition = transform.position;
            initialAngleZ = transform.rotation.eulerAngles.z;
            StartCoroutine(AttackBehavior());
        }

        // Đảm bảo cánh tay và vũ khí luôn cố định vị trí tương đối với Enemy
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

            // Tạo hiệu ứng xoay thân để mô phỏng đòn tấn công
            float attackAngle = Mathf.Sin(elapsed * attackFrequency * Mathf.PI * 2) * attackAmplitude;
            transform.rotation = Quaternion.Euler(0f, 0f, initialAngleZ + attackAngle);

            yield return null;
        }

        // Kết thúc tấn công, trả enemy về trạng thái ban đầu
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
            case EnemyType.Yellow:
                speed = 4f;
                break;
            case EnemyType.Boss1:
                speed = 3f;
                break;
            case EnemyType.Boss2:
                speed = 5f;
                break;
        }
    }
}
