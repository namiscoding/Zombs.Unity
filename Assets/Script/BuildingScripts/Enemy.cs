using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int health = 100;

    public void TakeDamage(int amount)
    {
        health -= amount;
        NotificationManager.Instance.ShowNotification($"enemy take {amount} damage");
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}