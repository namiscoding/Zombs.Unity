using UnityEngine;

public class Test : MonoBehaviour
{
    public TowerManager towerManager; // Assign in Inspector

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Press Space to select tower 0
        {
            towerManager.SelectBuilding(0);
        }
    }
}