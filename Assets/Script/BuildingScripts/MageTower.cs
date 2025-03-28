using UnityEngine;

public class MageTower : Tower
{
    protected override void Shoot()
    {
        if (towerData.projectilePrefab == null)
        {
            Debug.LogWarning("Projectile prefab is not set for this tower!");
            return;
        }

        // Calculate the direction to the target
        Vector3 direction = (target.transform.position - transform.position).normalized;
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Shoot three bullets: center, left, and right
        float angleOffset = towerData.angleBetweenBullets / 2; // Half the angle for left and right bullets

        // Center bullet
        ShootBullet(baseAngle);

        // Left bullet
        ShootBullet(baseAngle + angleOffset);

        // Right bullet
        ShootBullet(baseAngle - angleOffset);
    }

    private void ShootBullet(float angle)
    {
        Projectile projectile = projectilePool.GetProjectile(transform.position, Quaternion.Euler(0, 0, angle));
        if (projectile != null)
        {
            projectile.SetTarget(target, damage, towerData.projectileSpeed);
        }
    }
}