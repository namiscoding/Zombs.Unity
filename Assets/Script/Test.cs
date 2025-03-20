using UnityEngine;

public class Test : MonoBehaviour
{
    public BuildingManager buildingManager; // Assign in Inspector
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) FindAnyObjectByType<BuildingManager>().SelectBuilding(0); // Center  
        if (Input.GetKeyDown(KeyCode.Alpha2)) FindAnyObjectByType<BuildingManager>().SelectBuilding(1); // Tower  
        if (Input.GetKeyDown(KeyCode.Alpha3)) FindAnyObjectByType<BuildingManager>().SelectBuilding(2); // Building Type 3  
        if (Input.GetKeyDown(KeyCode.Alpha4)) FindAnyObjectByType<BuildingManager>().SelectBuilding(3); // Building Type 4  
        if (Input.GetKeyDown(KeyCode.Alpha5)) FindAnyObjectByType<BuildingManager>().SelectBuilding(4); // Building Type 5  
        if (Input.GetKeyDown(KeyCode.Alpha6)) FindAnyObjectByType<BuildingManager>().SelectBuilding(5); // Building Type 6  
        if (Input.GetKeyDown(KeyCode.Alpha7)) FindAnyObjectByType<BuildingManager>().SelectBuilding(6); // Building Type 7 
        if (Input.GetKeyDown(KeyCode.Alpha8)) FindAnyObjectByType<BuildingManager>().SelectBuilding(7);
    }
}