using UnityEngine;
using UnityEngine.EventSystems;

public class HealPanel : MonoBehaviour, IPointerClickHandler
{
    public PlayerManager playerManager;

    // Hồi máu khi nhấn vào Panel
    public void OnPointerClick(PointerEventData eventData)
    {
        if (playerManager != null)
        {
            if (playerManager != null)
            {
                //playerManager.FullHeal();
            }
        }
    }
}
