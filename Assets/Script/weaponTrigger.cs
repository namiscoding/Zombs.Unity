using UnityEngine;

public class weaponTrigger : MonoBehaviour
{
    public EnemyWithWeapon enemy;
    private bool hasHitPlayer = false; // Thêm biến để theo dõi đã đánh trúng player chưa

    void Start()
    {
        // Tìm Enemy là cha của Weapon
        enemy = GetComponentInParent<EnemyWithWeapon>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Weapon trigger detected collision with: {other.gameObject.name}");
        
        if (enemy == null)
        {
            Debug.Log("Enemy reference is null");
            return;
        }

        // Kiểm tra nếu đối tượng va chạm là Building (bao gồm cả Center)
        Building building = other.GetComponent<Building>();
        if (building != null)
        {
            Debug.Log($"Detected Building: {building.data.buildingName}");
            
            // Nếu là Center
            if (building is Center)
            {
                enemy.OnWeaponHitBase(other);
                Debug.Log($"Enemy đang tấn công Base");
            }
            // Nếu là công trình khác (bao gồm Barrier)
            else
            {
                enemy.OnWeaponHitBuilding(other);
                Debug.Log($"Enemy đang tấn công {building.data.buildingName}");
            }
            return;
        }

        // Nếu va chạm với Player, chỉ gây sát thương khi enemy đang trong trạng thái tấn công player
        if (other.CompareTag("Player") && !hasHitPlayer && enemy.currentState == EnemyState.AttackingPlayer)
        {
            hasHitPlayer = true;
            enemy.OnWeaponHitPlayer(other);
            // Reset biến hasHitPlayer sau một khoảng thời gian (thời gian cooldown của enemy)
            Invoke("ResetHitPlayer", enemy.attackCooldown);
        }
    }
    
    void ResetHitPlayer()
    {
        hasHitPlayer = false;
    }
}