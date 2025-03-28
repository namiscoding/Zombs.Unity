using UnityEngine;

public class Projectile : MonoBehaviour
{
    protected Enemy target;
    protected int damage;
    protected float speed;
    protected ProjectilePool pool;
    private SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Projectile prefab does not have a SpriteRenderer component!");
        }
    }

    public void SetPool(ProjectilePool pool)
    {
        this.pool = pool;
    }

    public void SetTarget(Enemy target, int damage, float speed)
    {
        this.target = target;
        this.damage = damage;
        this.speed = speed;
    }

    protected virtual void Update()
    {
        if (target == null)
        {
            ReturnToPool();
            return;
        }

        // Move towards the target
        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Rotate the projectile to face the target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Check if the projectile has reached the target
        if (Vector2.Distance(transform.position, target.transform.position) < 0.1f)
        {
            OnHit();
        }
    }

    protected virtual void OnHit()
    {
        if (target != null)
        {
            target.TakeDamage(damage);
        }
        ReturnToPool();
    }

    protected void ReturnToPool()
    {
        if (pool != null)
        {
            pool.ReturnProjectile(this);
        }
        else
        {
            Destroy(gameObject); // Fallback if pool is not set
        }
    }
}