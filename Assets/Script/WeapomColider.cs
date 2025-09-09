using System.Collections.Generic;
using UnityEngine;

public class WeapomColider : MonoBehaviour
{
    [SerializeField] private float damage;
    private PlayerManager playerManager;
    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();
    private WP_AxeManager wpAxeManager;
    private WP_SwordManager wpSwordManager;

    private void Start()
    {
        wpAxeManager = FindAnyObjectByType<WP_AxeManager>();
        wpSwordManager = FindAnyObjectByType<WP_SwordManager>();
        playerManager = GetComponentInParent<PlayerManager>();
        if (playerManager == null)
        {
            Debug.LogError("WeapomColider: PlayerManager not found in parent!");
        }
        UpdateDamageBasedOnWeapon();
    }

    private void OnEnable()
    {
        hitEnemies.Clear();
        UpdateDamageBasedOnWeapon();
        Debug.Log("WeapomColider: Enabled and damage set to " + damage);
    }

    private void UpdateDamageBasedOnWeapon()
    {
        if (playerManager != null)
        {
            switch (playerManager.GetCurrentWeaponState())
            {
                case PlayerManager.WeaponState.Axe:
                    if (wpAxeManager != null) damage = wpAxeManager.GetCurrentDamage();
                    Debug.Log("WeapomColider: Damage set to " + damage + " (Axe)");
                    break;
                case PlayerManager.WeaponState.Bow:
                    damage = 0f;
                    Debug.Log("WeapomColider: Damage set to " + damage + " (Bow)");
                    break;
                case PlayerManager.WeaponState.Sword:
                    if (wpSwordManager != null) damage = wpSwordManager.GetCurrentDamage();
                    Debug.Log("WeapomColider: Damage set to " + damage + " (Sword)");
                    break;
                default:
                    damage = 2f;
                    Debug.LogWarning("WeapomColider: Unknown weapon state, default damage: " + damage);
                    break;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("WeapomColider: Trigger entered with " + collision.gameObject.name + " (Tag: " + collision.tag + ")");
        if ((collision.CompareTag("bodyEnemy") || collision.CompareTag("Enemy")) && !hitEnemies.Contains(collision))
        {
            EnemyWithWeapon enemyWithWeapon = collision.GetComponent<EnemyWithWeapon>();
            if (enemyWithWeapon != null)
            {
                enemyWithWeapon.TakeDamage((int)damage);
                Debug.Log($"WeapomColider: Dealt {damage} damage to EnemyWithWeapon");
                hitEnemies.Add(collision);
            }

            EnemyNoWeapon enemyNoWeapon = collision.GetComponent<EnemyNoWeapon>();
            if (enemyNoWeapon != null)
            {
                enemyNoWeapon.TakeDamage((int)damage);
                Debug.Log($"WeapomColider: Dealt {damage} damage to EnemyNoWeapon");
                hitEnemies.Add(collision);
            }
        }
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
        Debug.Log("WeapomColider: Damage manually set to " + damage);
    }
}