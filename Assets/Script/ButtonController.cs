using UnityEngine;
using UnityEngine.UI; // ?? dùng Button

public class ButtonController : MonoBehaviour
{
    [SerializeField] private Button damageButton; // Tham chi?u ??n Button
    [SerializeField] private PlayerManager player; // Tham chi?u ??n PlayerMovement
    [SerializeField] private float damageAmount = 2f; // S? sát th??ng khi nh?n

    void Start()
    {
        // G?n s? ki?n nh?n button
        damageButton.onClick.AddListener(OnDamageButtonClicked);
    }

    void OnDamageButtonClicked()
    {
       
    }
}