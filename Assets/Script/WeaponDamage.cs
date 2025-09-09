using UnityEngine;

public class WeaponDamage : MonoBehaviour
{

    public int baseDamage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerManager player = collision.GetComponent<PlayerManager>();
            if (player != null && player.isAlive)
            {
                player.TakeDamage(baseDamage);
                Debug.Log("Player bị trúng đòn từ vũ khí và mất " + baseDamage + " máu.");
            }
        }
    }



}