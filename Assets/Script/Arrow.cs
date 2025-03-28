using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Camera mainCam;
    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();
    private Rigidbody2D rb; 
    public float force;
    WP_BowManager bowManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        // Xóa danh sách k? ??ch ?ã b? ?ánh khi b?t collider
        hitEnemies.Clear();
        //UpdateDamageBasedOnWeapon(); // C?p nh?t l?i damage khi collider ???c b?t
    }
    void Start()
    {
        bowManager = FindFirstObjectByType<WP_BowManager>();
        if (bowManager == null)
        {
            Debug.LogError("WP_BowManager not found in the scene! Make sure it's active.");
            return;
        }

        
        mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("MainCamera not found! Make sure your camera has the tag 'MainCamera'.");
            return;
        }

        
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is missing on the bullet!");
            return;
        }

        force = bowManager.GetCurrentDamage(); 
        Debug.Log("Force: " + force);

        // Get mouse position
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Ensure it's in 2D

        // Calculate direction
        Vector3 direction = (mousePos - transform.position).normalized;

        // Apply force (if velocity didn't work)
        rb.AddForce(direction * force, ForceMode2D.Impulse); // Impulse applies immediate force

        // Set rotation
        float rot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot);
        
        Debug.Log("Bullet instantiated and moving towards " + mousePos);

        //Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ki?m tra va ch?m v?i k? ??ch
        if ((collision.CompareTag("bodyEnemy") || collision.CompareTag("Enemy")) && !hitEnemies.Contains(collision))
        {
            // Ki?m tra va ch?m v?i EnemyWithWeapon
            EnemyWithWeapon enemyWithWeapon = collision.GetComponent<EnemyWithWeapon>();
            if (enemyWithWeapon != null)
            {
                enemyWithWeapon.TakeDamage((int)force);
                Debug.Log($"WeaponCollider: ?ã gây {force} sát th??ng cho EnemyWithWeapon");
                hitEnemies.Add(collision);
            }

            // Ki?m tra va ch?m v?i EnemyNoWeapon
            EnemyNoWeapon enemyNoWeapon = collision.GetComponent<EnemyNoWeapon>();
            if (enemyNoWeapon != null)
            {
                enemyNoWeapon.TakeDamage((int)force);
                Debug.Log($"WeaponCollider: ?ã gây {force} sát th??ng cho EnemyNoWeapon");
                hitEnemies.Add(collision);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
