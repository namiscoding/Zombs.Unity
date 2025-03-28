using UnityEngine;

public class BombProjectile : Projectile
{
    private float explosionRadius;

    public void SetExplosionData(float explosionRadius)
    {
        this.explosionRadius = explosionRadius;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // Check for EnemyNoWeapon
        EnemyNoWeapon enemyNoWeapon = other.GetComponent<EnemyNoWeapon>();
        if (enemyNoWeapon != null)
        {
            OnHit(enemyNoWeapon, null);
            return;
        }

        // Check for EnemyWithWeapon
        EnemyWithWeapon enemyWithWeapon = other.GetComponent<EnemyWithWeapon>();
        if (enemyWithWeapon != null)
        {
            OnHit(null, enemyWithWeapon);
        }
    }

    protected override void OnHit(EnemyNoWeapon hitEnemyNoWeapon, EnemyWithWeapon hitEnemyWithWeapon)
    {
        // Apply damage to the primary target
        if (hitEnemyNoWeapon != null)
        {
            hitEnemyNoWeapon.TakeDamage(damage);
        }
        else if (hitEnemyWithWeapon != null)
        {
            hitEnemyWithWeapon.TakeDamage(damage);
        }

        // Find all enemies within the explosion radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hits)
        {
            // Check for EnemyNoWeapon
            EnemyNoWeapon enemyNoWeapon = hit.GetComponent<EnemyNoWeapon>();
            if (enemyNoWeapon != null && enemyNoWeapon != hitEnemyNoWeapon)
            {
                enemyNoWeapon.TakeDamage(damage); // Explosion damage is the same as primary damage
                continue;
            }

            // Check for EnemyWithWeapon
            EnemyWithWeapon enemyWithWeapon = hit.GetComponent<EnemyWithWeapon>();
            if (enemyWithWeapon != null && enemyWithWeapon != hitEnemyWithWeapon)
            {
                enemyWithWeapon.TakeDamage(damage); // Explosion damage is the same as primary damage
            }
        }

        ReturnToPool();
    }
}