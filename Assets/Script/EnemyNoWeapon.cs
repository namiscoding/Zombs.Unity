using UnityEngine;
using UnityEngine.UI; // To use UI elements like Slider for health bar
using System.Collections;

public class EnemyNoWeapon : MonoBehaviour
{
    public enum EnemyType { Blue, Green, Red, Yellow }
    public EnemyType enemyType;
    public float speed;
    public int maxHealth;
    private int currentHealth;
    public int attackDamage; // 💥 Attack damage when enemy hits

    public float stopDistance = 0.5f;  // Distance at which the enemy will stop
    public float attackAmplitude = 2f;
    public float attackFrequency = 1f;
    public float attackDuration = 1f;

    private Transform player;
    private PlayerManager playerManager; // Reference to PlayerManager
    private Rigidbody2D rb;
    private bool isAttacking = false;
    private bool hasHitPlayer = false;  // Flag to track if the enemy has already hit the player
    private float initialAngleZ;
    private Vector3 initialPosition;

    public Transform armTransform;
    private Vector3 armLocalPosition;
    public Slider healthBar;
    private Image healthBarFill;

    void Start()
    {
        SetAttributesBasedOnType();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            playerManager = player.GetComponent<PlayerManager>();
        }

        rb = GetComponent<Rigidbody2D>() ?? gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (armTransform != null)
            armLocalPosition = armTransform.localPosition;

        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
            healthBarFill = healthBar.fillRect.GetComponent<Image>(); // Get `Image` of Fill
            healthBarFill.color = Color.green; // Initial color
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 direction = (player.position - transform.position).normalized;
        RotateTowardsPlayer(direction);

        if (distanceToPlayer > stopDistance && !isAttacking)
        {
            rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
        }
        else if (distanceToPlayer <= stopDistance && !isAttacking)
        {
            isAttacking = true;
            initialPosition = transform.position;
            initialAngleZ = transform.rotation.eulerAngles.z;
            StartCoroutine(AttackBehavior());
        }
    }

    void RotateTowardsPlayer(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    IEnumerator AttackBehavior()
    {
        float elapsed = 0f;
        while (elapsed < attackDuration)
        {
            elapsed += Time.deltaTime;
            float attackAngle = Mathf.Sin(elapsed * attackFrequency * Mathf.PI * 2) * attackAmplitude;
            transform.rotation = Quaternion.Euler(0f, 0f, initialAngleZ + attackAngle);
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0f, 0f, initialAngleZ);
        isAttacking = false;
    }

    // Method to handle collision with the player
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasHitPlayer)
        {
            // Only apply damage when the player is hit, and ensure damage is only applied once per collision
            if (playerManager != null)
            {
                // Apply a small amount of damage each time the player is hit
              // You can adjust this value as needed
                playerManager.TakeDamage(attackDamage); // Deal damage to player
                hasHitPlayer = true; // Flag to prevent multiple damage in the same collision
                Debug.Log($"{enemyType} hit the Player! Causing {attackDamage} damage.");
            }
        }
    }

    // Reset the flag when the player leaves the trigger
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hasHitPlayer = false;  // Reset flag when player leaves collision area
        }
    }

    void SetAttributesBasedOnType()
    {
        switch (enemyType)
        {
            case EnemyType.Blue:
                speed = 3f;
                maxHealth = 100;
                attackDamage = 4;
                break;
            case EnemyType.Green:
                speed = 2f;
                maxHealth = 120;
                attackDamage = 3;
                break;
            case EnemyType.Red:
                speed = 6f;
                maxHealth = 200;
                attackDamage = 5;
                break;
            case EnemyType.Yellow:
                speed = 4f;
                maxHealth = 150;
                attackDamage = 6;
                break;
        }
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{enemyType} took {damage} damage. Remaining health: {currentHealth}");

        if (healthBar != null)
        {
            healthBar.value = currentHealth;

            if (healthBarFill != null)
            {
                if (currentHealth > maxHealth * 0.6f)
                    healthBarFill.color = Color.green; // 💚
                else if (currentHealth > maxHealth * 0.3f)
                    healthBarFill.color = Color.yellow; // 💛
                else
                    healthBarFill.color = Color.red; // ❤️
            }
        }

        if (currentHealth <= 0)
        {
            healthBar.gameObject.SetActive(false); // Optionally hide the health bar when dead
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{enemyType} has been defeated!");
        gameObject.SetActive(false); // Set to inactive instead of destroying the object
    }
}
