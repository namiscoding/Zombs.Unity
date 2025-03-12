
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int wood;          // Số lượng gỗ hiện tại
    [SerializeField] private int stone;         // Số lượng đá hiện tại
    [SerializeField] private Text txtWood;      // Text UI hiển thị số lượng gỗ
    [SerializeField] private Text txtStone;     // Text UI hiển thị số lượng đá

    void Start()
    {
        UpdateResourceUI();
    }

    void UpdateResourceUI()
    {
        txtWood.text = "" + wood;
        txtStone.text = "" + stone;
        //txtWood.text = "Wood: " + wood;
        //txtStone.text = "Stone: " + stone;
    }

    // Thêm đá
    public void AddStone(int quality)
    {
        stone += quality;      // Cộng số lượng đá
        UpdateResourceUI();    // Cập nhật lại UI
    }

    // Thêm gỗ
    public void AddWood(int quality)
    {
        wood += quality;       // Cộng số lượng gỗ
        UpdateResourceUI();    // Cập nhật lại UI
    }
}
