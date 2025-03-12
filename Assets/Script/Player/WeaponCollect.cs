using UnityEngine;

public class WeaponCollect : MonoBehaviour
{
    [SerializeField] private Transform attackSpawnPoint;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private ObjectPool attackPool;
    private float lastAttackTime = 0f;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (attackPool == null)
        {
            attackPool = FindAnyObjectByType<ObjectPool>();
        }
    }

    void Update()
    {
        HandleAttack();
    }

    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
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

                animator.SetBool("isCollect", true);
                Invoke("ResetAnimation", 0.2f);

                return true;
            }
        }
        return false;
    }

    void ResetAnimation()
    {
        animator.SetBool("isCollect", false);
    }
}
