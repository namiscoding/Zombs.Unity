using UnityEngine;

public class BombProjectile : Projectile
{
    private float explosionRadius;

    public void SetExplosionData(float explosionRadius)
    {
        this.explosionRadius = explosionRadius;
    }

    protected override void OnHit()
    {
        // Apply damage to the primary target
        if (target != null)
        {
            target.TakeDamage(damage);
        }

        // Find all enemies within the explosion radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null && enemy != target) // Exclude the primary target
            {
                enemy.TakeDamage(damage); // Explosion damage is the same as primary damage
            }
        }

        ReturnToPool();
    }
}