using UnityEngine;

public class WeaponCollect : MonoBehaviour
{
    [SerializeField] private Transform attackSpawnPoint;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private ObjectPool attackPool;
    private float lastAttackTime = 0f;
    private Animator animator;
    private PlayerManager playerManager;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerManager = GetComponent<PlayerManager>();
        if (attackPool == null)
        {
            attackPool = FindAnyObjectByType<ObjectPool>();
        }
        if (playerManager == null)
        {
            Debug.LogError("WeaponCollect: PlayerManager not found on this GameObject!");
        }
    }

    void Update()
    {
        HandleAttack();
    }

    void HandleAttack()
    {
        if (Input.GetMouseButton(1) && Time.time >= lastAttackTime + attackCooldown)
        {
            bool attackSuccess = Shoot();
            if (attackSuccess)
            {
                lastAttackTime = Time.time;
            }
        }
    }

    bool Shoot()
    {
        if (attackPool != null && attackSpawnPoint != null)
        {
            GameObject attack = attackPool.GetObject();
            if (attack != null)
            {
                attack.transform.position = attackSpawnPoint.position;
                attack.transform.rotation = attackSpawnPoint.rotation;
                attack.SetActive(true);

                PlayerCollect collect = attack.GetComponent<PlayerCollect>();
                if (collect != null)
                {
                    collect.SetPool(attackPool);
                }

                WeapomColider collider = attack.GetComponent<WeapomColider>(); // Sử dụng WeapomColider
                if (collider != null && playerManager != null)
                {
                    switch (playerManager.GetCurrentWeaponState())
                    {
                        case PlayerManager.WeaponState.Axe:
                            WP_AxeManager axeManager = FindAnyObjectByType<WP_AxeManager>();
                            if (axeManager != null) collider.SetDamage(axeManager.GetCurrentDamage());
                            break;
                        case PlayerManager.WeaponState.Sword:
                            WP_SwordManager swordManager = FindAnyObjectByType<WP_SwordManager>();
                            if (swordManager != null) collider.SetDamage(swordManager.GetCurrentDamage());
                            break;
                    }
                    Debug.Log("WeaponCollect: Spawned attack object with damage set");
                }
                else
                {
                    Debug.LogError("WeaponCollect: WeapomColider not found on spawned object!");
                }

                return true;
            }
            else
            {
                Debug.LogWarning("WeaponCollect: No object available in pool!");
            }
        }
        else
        {
            Debug.LogError("WeaponCollect: attackPool or attackSpawnPoint is null!");
        }
        return false;
    }
}