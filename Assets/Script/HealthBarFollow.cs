using UnityEngine;

public class HealthBarFollow : MonoBehaviour
{
    public Transform enemy; // Tham chiếu đến kẻ thù (Enemy)
    public Vector3 offset;  // Để thay đổi vị trí thanh máu (ví dụ: di chuyển lên trên đầu kẻ thù)

    void Update()
    {
        // Cập nhật vị trí của thanh máu sao cho nó di chuyển theo kẻ thù
        // Đồng thời giữ cho thanh máu không xoay theo kẻ thù
        transform.position = enemy.position + offset;
        transform.rotation = Quaternion.identity; // Đảm bảo thanh máu không xoay
    }
}
