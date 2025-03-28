using UnityEngine;

public class BombTower : Tower
{
    protected override void Shoot()
    {
        if (towerData.projectilePrefab == null)
        {
            Debug.LogWarning("Projectile prefab is not set for this tower!");
            return;
        }

        // Get a bomb projectile from the pool
        BombProjectile projectile = projectilePool.GetProjectile(transform.position, Quaternion.identity) as BombProjectile;
        if (projectile != null)
        {
            projectile.SetTarget(target, damage, towerData.projectileSpeed);
            projectile.SetExplosionData(towerData.explosionRadius); // Only pass explosionRadius
        }
    }
}