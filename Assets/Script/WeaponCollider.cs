using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour
{
    [SerializeField] private float damage = 2f;
    private PlayerManager playerManager;
    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();

    private void Start()
    {
        playerManager = GetComponentInParent<PlayerManager>();
        damage = playerManager.spinAttackDamage;
    }

    private void OnEnable()
    {
        // Xóa danh sách kẻ địch đã bị đánh khi bật collider
        hitEnemies.Clear();
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