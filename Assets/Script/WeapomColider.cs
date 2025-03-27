using System.Collections.Generic;
using UnityEngine;

public class WeapomColider : MonoBehaviour
{
    [SerializeField] private float damage; // Giá trị mặc định
    private PlayerManager playerManager;
    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();
    private WP_AxeManager WP_AxeManager;
    private WP_SwordManager WP_swordManager;
    private void Start()
    {
        WP_AxeManager = FindAnyObjectByType<WP_AxeManager>();
        WP_swordManager = FindAnyObjectByType<WP_SwordManager>();
        playerManager = GetComponentInParent<PlayerManager>();
        if (playerManager == null)
        {
            Debug.LogError("PlayerManager not found in parent!");
        }
        UpdateDamageBasedOnWeapon(); // Cập nhật damage ban đầu dựa trên vũ khí
    }

    private void OnEnable()
    {
        // Xóa danh sách kẻ địch đã bị đánh khi bật collider
        hitEnemies.Clear();
        UpdateDamageBasedOnWeapon(); // Cập nhật lại damage khi collider được bật
    }

    private void UpdateDamageBasedOnWeapon()
    {
        if (playerManager != null)
        {
            switch (playerManager.GetCurrentWeaponState())
            {
                case PlayerManager.WeaponState.Axe:
                    damage = WP_AxeManager.GetCurrentDamage();
                    Debug.Log("WeaponCollider: Damage set to " + damage);
                    break;
                case PlayerManager.WeaponState.Bow:
                    damage = 0f;
                    Debug.Log("WeaponCollider: Damage set to " + damage);
                    break;
                case PlayerManager.WeaponState.Sword:
                    damage = WP_swordManager.GetCurrentDamage();
                    Debug.Log("WeaponCollider: Damage set to 5 " + damage);
                    break;
                default:
                    damage = 2f; // Giá trị mặc định nếu có lỗi
                    Debug.LogWarning("WeaponCollider: Unknown weapon state" + damage);
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra va chạm với kẻ địch
        if ((collision.CompareTag("bodyEnemy") || collision.CompareTag("Enemy")) && !hitEnemies.Contains(collision))
        {
            // Kiểm tra va chạm với EnemyWithWeapon
            EnemyWithWeapon enemyWithWeapon = collision.GetComponent<EnemyWithWeapon>();
            if (enemyWithWeapon != null)
            {
                enemyWithWeapon.TakeDamage((int)damage);
                Debug.Log($"WeaponCollider: Đã gây {damage} sát thương cho EnemyWithWeapon");
                hitEnemies.Add(collision);
            }

            // Kiểm tra va chạm với EnemyNoWeapon
            EnemyNoWeapon enemyNoWeapon = collision.GetComponent<EnemyNoWeapon>();
            if (enemyNoWeapon != null)
            {
                enemyNoWeapon.TakeDamage((int)damage);
                Debug.Log($"WeaponCollider: Đã gây {damage} sát thương cho EnemyNoWeapon");
                hitEnemies.Add(collision);
            }
        }
    }
}