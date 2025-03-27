using UnityEngine;
using UnityEngine.UI;

public class EnemyWithWeapon : MonoBehaviour
{
    public enum EnemyType { Blue, Green, Red, Yellow, Boss1, Boss2 }
    public EnemyType enemyType;
    public enum WeaponType { Sword, Mace, Shovel, NoWeapon, Boss1, Boss2 }
    public WeaponType weaponType;
    public float speed;
    public int maxHealth = 10;
    public int currentHealth=4;
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

    // Thêm biến để theo dõi trạng thái và mục tiêu
    public EnemyState currentState = EnemyState.AttackingBase;
    private Transform baseTransform;
    private bool wasAttackedByPlayer = false;
    private float timeToForgetPlayer = 5f; // Thời gian quên player nếu không bị tấn công
    private float lastTimeAttackedByPlayer = 0f;

    public Transform armTransform;
    private Vector3 armLocalPosition;

    // Add a reference to the current target building
    private Building targetBuilding;
    private float buildingDetectionRadius = 5f; // Radius to detect buildings
    private float buildingCheckInterval = 0.5f; // How often to check for buildings
    private float lastBuildingCheckTime = 0f;

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

        // Tìm base trong scene
        Center center = FindObjectOfType<Center>();
        if (center != null)
        {
            baseTransform = center.transform;
            currentState = EnemyState.AttackingBase;
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Base trong scene!");
            currentState = EnemyState.Idle;
        }

        rb = GetComponent<Rigidbody2D>() ?? gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

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

        // Initial building check
        CheckForNearbyBuildings();
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
    [SerializeField]
    private float rotationAngle = 50f; // Angle offset that can be adjusted in the editor
    
    void RotateTowardsTarget()
    {
        // Calculate the angle to face the target
        Vector3 targetPosition = Vector3.zero;
        
        if (currentState == EnemyState.AttackingBuilding)
        {
            targetPosition = targetBuilding.transform.position;
        }
        else if (currentState == EnemyState.AttackingPlayer)
        {
            targetPosition = player.transform.position;
        }
        else if (currentState == EnemyState.AttackingBase)
        {
            targetPosition = baseTransform.position;
        }
        else
        {
            print("No target to rotate towards");
        }
        
        if (targetPosition != Vector3.zero)
        {
            Vector3 direction = targetPosition - transform.position;
            // Make the enemy face the target (considering -x is forward for the enemy)
            // We use -90f to adjust for sprite orientation
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationAngle  ;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15);
        }
    }

    void Update()
    {
        // RotateTowardsTarget();
        // Check for buildings periodically
        if (Time.time - lastBuildingCheckTime > buildingCheckInterval)
        {
            CheckForNearbyBuildings();
            lastBuildingCheckTime = Time.time;
        }

        // Kiểm tra nếu player đã chết và enemy đang tấn công player
        if (currentState == EnemyState.AttackingPlayer && (player == null || !playerManager.isAlive))
        {
            // Check for buildings first, otherwise attack base
            if (!CheckForNearbyBuildings())
            {
                currentState = EnemyState.AttackingBase;
                Debug.Log($"{enemyType} chuyển sang tấn công Base vì Player đã chết");
            }
            isAttacking = false;
            wasAttackedByPlayer = false;
        }

        // Kiểm tra nếu đã quá thời gian quên player
        if (wasAttackedByPlayer && Time.time - lastTimeAttackedByPlayer > timeToForgetPlayer)
        {
            wasAttackedByPlayer = false;
            if (currentState == EnemyState.AttackingPlayer)
            {
                // Check for buildings first, otherwise attack base
                if (!CheckForNearbyBuildings())
                {
                    currentState = EnemyState.AttackingBase;
                    Debug.Log($"{enemyType} quên Player và quay lại tấn công Base");
                }
                isAttacking = false;
            }
        }

        // Check if target building was destroyed
        if (currentState == EnemyState.AttackingBuilding && targetBuilding == null)
        {
            // Building was destroyed, find another or attack base
            if (!CheckForNearbyBuildings())
            {
                currentState = EnemyState.AttackingBase;
                Debug.Log($"{enemyType} chuyển sang tấn công Base vì công trình đã bị phá hủy");
            }
            isAttacking = false;
        }

        // Xác định mục tiêu dựa trên trạng thái hiện tại
        Transform currentTarget = null;
        
        switch (currentState)
        {
            case EnemyState.AttackingPlayer:
                currentTarget = player;
                break;
            case EnemyState.AttackingBuilding:
                currentTarget = targetBuilding?.transform;
                break;
            case EnemyState.AttackingBase:
                currentTarget = baseTransform;
                break;
            default:
                currentTarget = null;
                break;
        }
        
        // Nếu không có mục tiêu hợp lệ, chuyển sang trạng thái khác
        if (currentTarget == null)
        {
            // Try to find a building first
            if (CheckForNearbyBuildings())
            {
                currentTarget = targetBuilding.transform;
            }
            // If no building, check if base is available
            else if (baseTransform != null)
            {
                currentState = EnemyState.AttackingBase;
                currentTarget = baseTransform;
            }
            // If no base, check if player is available
            else if (player != null && playerManager.isAlive)
            {
                currentState = EnemyState.AttackingPlayer;
                currentTarget = player;
            }
            else
            {
                currentState = EnemyState.Idle;
                return; // Không có mục tiêu nào, không làm gì cả
            }
        }

        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);
        Vector2 direction = (currentTarget.position - transform.position).normalized;

        // Nếu mục tiêu chạy xa ra thì cho phép enemy đuổi tiếp
        if (distanceToTarget > stopDistance + 0.5f)
        {
            isAttacking = false;
            animator?.SetBool("isNearPlayer", false);
        }

        if (distanceToTarget > stopDistance && !isAttacking)
        {
            // Kiểm tra trước khi di chuyển
            Vector2 newPosition = rb.position + direction * speed * Time.deltaTime;
            RaycastHit2D hit = Physics2D.Raycast(rb.position, direction, speed * Time.deltaTime);
            
            if (hit.collider != null)
            {
                Building building = hit.collider.GetComponent<Building>();
                if (building != null && !(building is Center))
                {
                    // Nếu phát hiện building, dừng lại và bắt đầu tấn công
                    isAttacking = true;
                    animator?.SetBool("isNearPlayer", true);
                    Debug.Log($"{enemyType} phát hiện va chạm với {building.data.buildingName} và dừng lại");
                }
                else
                {
                    rb.MovePosition(newPosition);
                }
            }
            else
            {
                rb.MovePosition(newPosition);
            }
            
            animator?.SetBool("isNearPlayer", false);
        }
        else if (distanceToTarget <= attackRange)
        {
            // Luôn kích hoạt animation khi trong tầm tấn công, bất kể mục tiêu là gì
            animator?.SetBool("isNearPlayer", true);
        }
        RotateTowardsTarget();
    }

    void SetAttributesBasedOnType()
    {
        switch (enemyType)
        {
            case EnemyType.Blue:
                speed = 3f;
                maxHealth = 120; // Tăng một chút sức khỏe để kẻ địch này bền bỉ hơn
                break;
            case EnemyType.Green:
                speed = 2f;
                maxHealth = 150; // Tăng sức khỏe của Green Enemy, khiến nó khó bị hạ gục hơn
                break;
            case EnemyType.Red:
                speed = 6f;
                maxHealth = 100; // Tăng sức khỏe một chút để Red Enemy không dễ bị tiêu diệt
                break;
            case EnemyType.Yellow:
                speed = 4f;
                maxHealth = 180; // Tăng sức khỏe để tạo thử thách khi người chơi đối phó
                break;
            case EnemyType.Boss1:
                speed = 3f;
                maxHealth = 350; // Boss đầu tiên mạnh mẽ hơn, cần nhiều đòn tấn công để hạ gục
                break;
            case EnemyType.Boss2:
                speed = 5f;
                maxHealth = 500; // Boss thứ hai cực kỳ mạnh mẽ và khó đánh bại hơn
                break;
        }
        currentHealth = maxHealth;  // Đặt lại sức khỏe hiện tại sau khi thay đổi maxHealth
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{enemyType} nhận {damage} sát thương. Máu còn lại: {currentHealth}");

        // Khi bị tấn công bởi player, chuyển sang tấn công player
        wasAttackedByPlayer = true;
        lastTimeAttackedByPlayer = Time.time;
        
        if (currentState == EnemyState.AttackingBase || currentState == EnemyState.AttackingBuilding)
        {
            currentState = EnemyState.AttackingPlayer;
            isAttacking = false; // Reset trạng thái tấn công để bắt đầu đuổi theo player
            Debug.Log($"{enemyType} chuyển sang tấn công Player vì bị tấn công");
        }

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
        }
        
        if (currentHealth <= 0)
        {
            if (healthBar != null)
            {
                healthBar.gameObject.SetActive(false);
            }
            Die();
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

    // Thêm phương thức để gây sát thương cho base
    public void OnWeaponHitBase(Collider2D other)
    {
        Center center = other.GetComponent<Center>();
        if (center != null && Time.time - lastAttackTime >= attackCooldown)
        {
            center.TakeDamage(baseDamage);
            Debug.Log($"{enemyType}'s weapon gây {baseDamage} sát thương cho Base!");
            lastAttackTime = Time.time;
        }
    }

    // Method to check for nearby buildings and set as target if found
    private bool CheckForNearbyBuildings()
    {
        // Find all buildings in detection radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, buildingDetectionRadius);
        
        Debug.Log($"Found {colliders.Length} colliders in detection radius");
        
        Building nearestBuilding = null;
        float nearestDistance = float.MaxValue;
        
        foreach (Collider2D collider in colliders)
        {
            Building building = collider.GetComponent<Building>();
            
            // Skip if not a building or if it's the Center (base)
            if (building == null)
            {
                Debug.Log($"Collider {collider.gameObject.name} is not a building");
                continue;
            }
            
            if (building is Center)
            {
                Debug.Log($"Collider {collider.gameObject.name} is the Center, skipping");
                continue;
            }
                
            float distance = Vector2.Distance(transform.position, building.transform.position);
            Debug.Log($"Found building: {building.data.buildingName} at distance {distance}");
            
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestBuilding = building;
            }
        }
        
        if (nearestBuilding != null)
        {
            targetBuilding = nearestBuilding;
            currentState = EnemyState.AttackingBuilding;
            Debug.Log($"{enemyType} phát hiện công trình {targetBuilding.data.buildingName} và chuyển sang tấn công");
            return true;
        }
        
        targetBuilding = null;
        return false;
    }

    // Add method to handle weapon hitting buildings
    public void OnWeaponHitBuilding(Collider2D other)
    {
        Building building = other.GetComponent<Building>();
        if (building != null && Time.time - lastAttackTime >= attackCooldown)
        {
            building.TakeDamage(baseDamage);
            Debug.Log($"{enemyType}'s weapon gây {baseDamage} sát thương cho {building.data.buildingName}!");
            lastAttackTime = Time.time;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra va chạm với building
        Building building = collision.gameObject.GetComponent<Building>();
        if (building != null && !(building is Center))
        {
            // Dừng lại và bắt đầu tấn công khi va chạm với building
            isAttacking = true;
            animator?.SetBool("isNearPlayer", true);
            Debug.Log($"{enemyType} va chạm với {building.data.buildingName} và dừng lại");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Không làm gì khi va chạm với player
    }
}
