using UnityEngine;

public class LockCanvas : MonoBehaviour
{
    void Start()
    {
        // Đảm bảo Canvas bắt đầu không bị xoay
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        // Cập nhật mỗi frame để Canvas không bị xoay hay di chuyển
        transform.rotation = Quaternion.identity;
    }
}
