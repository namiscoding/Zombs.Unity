using UnityEngine;

public abstract class Tower : Building
{
    protected TowerData towerData; // Tower-specific data
    protected float range; // Current range (updated with level)
    protected float fireRate; // Current fire rate (shots per second)
    protected int damage; // Current damage per shot
    protected float fireCooldown; // Time until the next shot can be fired
    protected EnemyNoWeapon targetNoWeapon; // Current target if EnemyNoWeapon
    protected EnemyWithWeapon targetWithWeapon; // Current target if EnemyWithWeapon
    protected ProjectilePool projectilePool; // Pool for projectiles

    protected override void Start()
    {
        base.Start();
        towerData = data as TowerData;
        if (towerData == null)
        {
            Debug.LogError("TowerData is not set or is not a TowerData type!");
            return;
        }
        UpdateStats(); // Initialize range, fire rate, and damage
        fireCooldown = 0f;

        // Initialize the projectile pool
        GameObject poolObj = new GameObject($"{gameObject.name}_ProjectilePool");
        projectilePool = poolObj.AddComponent<ProjectilePool>();
        projectilePool.Initialize(towerData.projectilePrefab, 20); // Pool size of 20 projectiles
    }

    protected override void Update()
    {
        base.Update();
        if (fireCooldown > 0)
        {
            fireCooldown -= Time.deltaTime;
        }

        // Find the nearest enemy in range
        FindNearestEnemyInRange();
        if ((targetNoWeapon != null || targetWithWeapon != null) && fireCooldown <= 0)
        {
            Shoot();
            fireCooldown = 1f / fireRate; // Reset cooldown based on fire rate
        }
    }

    protected override void UpdateStats()
    {
        base.UpdateStats();
        if (towerData != null)
        {
            range = towerData.baseRange * towerData.rangeMultipliers[currentLevel - 1];
            fireRate = towerData.baseFireRate * towerData.fireRateMultipliers[currentLevel - 1];
            damage = (int)(towerData.baseDamage * towerData.damageMultipliers[currentLevel - 1]);
        }
    }

    private void FindNearestEnemyInRange()
    {
        targetNoWeapon = null;
        targetWithWeapon = null;
        float nearestDistance = float.MaxValue;

        // Find all EnemyNoWeapon in the scene
        EnemyNoWeapon[] enemiesNoWeapon = FindObjectsOfType<EnemyNoWeapon>();
        foreach (EnemyNoWeapon enemy in enemiesNoWeapon)
        {
            if (enemy == null) continue;

            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance <= range && distance < nearestDistance)
            {
                nearestDistance = distance;
                targetNoWeapon = enemy;
                targetWithWeapon = null; // Reset the other target
            }
        }

        // Find all EnemyWithWeapon in the scene
        EnemyWithWeapon[] enemiesWithWeapon = FindObjectsOfType<EnemyWithWeapon>();
        foreach (EnemyWithWeapon enemy in enemiesWithWeapon)
        {
            if (enemy == null) continue;

            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance <= range && distance < nearestDistance)
            {
                nearestDistance = distance;
                targetNoWeapon = null; // Reset the other target
                targetWithWeapon = enemy;
            }
        }
    }

    protected virtual void Shoot()
    {
        if (towerData.projectilePrefab == null)
        {
            Debug.LogWarning("Projectile prefab is not set for this tower!");
            return;
        }

        // Get a projectile from the pool
        Projectile projectile = projectilePool.GetProjectile(transform.position, Quaternion.identity);
        if (projectile != null)
        {
            projectile.SetTarget(targetNoWeapon, targetWithWeapon, damage, towerData.projectileSpeed);
        }
    }
}