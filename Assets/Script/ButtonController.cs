using UnityEngine;
using UnityEngine.UI; // ?? d�ng Button

public class ButtonController : MonoBehaviour
{
    [SerializeField] private Button damageButton; // Tham chi?u ??n Button
    [SerializeField] private PlayerManager player; // Tham chi?u ??n PlayerMovement
    [SerializeField] private float damageAmount = 2f; // S? s�t th??ng khi nh?n

    void Start()
    {
        // G?n s? ki?n nh?n button
        damageButton.onClick.AddListener(OnDamageButtonClicked);
    }

    void OnDamageButtonClicked()
    {
        if (player != null)
        {
            player.TakeDamage(damageAmount); // G?i h�m m?t m�u c?a Player
        }
        else
        {
            Debug.LogError("Player reference is missing!");
        }
    }
}