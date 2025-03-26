using UnityEngine;

public class weaponTrigger : MonoBehaviour
{
    private EnemyWithWeapon enemy;

    void Start()
    {
        // Tìm Enemy là cha của Weapon
        enemy = GetComponentInParent<EnemyWithWeapon>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Nếu va chạm với Player
        if (other.CompareTag("Player") && enemy != null)
        {
            enemy.OnWeaponHitPlayer(other);
        }
    }
}