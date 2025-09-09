using UnityEngine;

public class Projectile : MonoBehaviour
{
    protected EnemyNoWeapon targetNoWeapon; // Target if it's an EnemyNoWeapon
    protected EnemyWithWeapon targetWithWeapon; // Target if it's an EnemyWithWeapon
    protected Vector3 targetPosition; // Fixed position to move towards
    protected int damage;
    protected float speed;
    protected bool keepSpriteVertical; // If true, keep the sprite vertical
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

    public void SetTarget(EnemyNoWeapon targetNoWeapon, EnemyWithWeapon targetWithWeapon, int damage, float speed)
    {
        this.targetNoWeapon = targetNoWeapon;
        this.targetWithWeapon = targetWithWeapon;
        this.damage = damage;
        this.speed = speed;

        // Set the target position once based on the current position of the target
        if (targetNoWeapon != null)
        {
            targetPosition = targetNoWeapon.transform.position;
        }
        else if (targetWithWeapon != null)
        {
            targetPosition = targetWithWeapon.transform.position;
        }
        else
        {
            targetPosition = transform.position; // Fallback to current position
        }

        // Set the initial rotation based on the target position
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.rotation = Quaternion.Euler(0, 0, 90); 
        
    }

    protected virtual void Update()
    {
        // Move towards the fixed target position
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Check if the projectile has reached the target position
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            ReturnToPool();
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
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

    protected virtual void OnHit(EnemyNoWeapon hitEnemyNoWeapon, EnemyWithWeapon hitEnemyWithWeapon)
    {
        if (hitEnemyNoWeapon != null)
        {
            hitEnemyNoWeapon.TakeDamage(damage);
        }
        else if (hitEnemyWithWeapon != null)
        {
            hitEnemyWithWeapon.TakeDamage(damage);
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