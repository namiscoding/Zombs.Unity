using UnityEngine;
using System.Collections.Generic;

public class TowerManager : MonoBehaviour
{
    public TowerData[] availableTowers;
    private TowerData selectedTower = null;
    private GameObject towerPreview = null;
    public GameObject[] towerPrefabs;
    private Camera mainCamera;
    private Dictionary<string, int> towerCounts = new Dictionary<string, int>();

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("No Main Camera found in the scene!");
            mainCamera = FindFirstObjectByType<Camera>();
        }
        foreach (var tower in availableTowers)
        {
            towerCounts[tower.towerName] = 0;
        }
    }

    void Update()
    {
        if (selectedTower != null)
        {
            if (towerPreview == null)
            {
                Debug.LogWarning("towerPreview was null in Update, recreating...");
                CreatePreview();
            }
            if (towerPreview != null)
            {
                HandleBuildingPlacement();
            }
        }
    }

    public void SelectBuilding(int buildingIndex)
    {
        if (buildingIndex >= 0 && buildingIndex < availableTowers.Length)
        {
            selectedTower = availableTowers[buildingIndex];
            if (selectedTower == null || selectedTower.sprite == null)
            {
                Debug.LogError($"Invalid TowerData at index {buildingIndex} or missing sprite!");
                selectedTower = null;
                return;
            }
            if (towerCounts[selectedTower.towerName] >= selectedTower.maxQuantity)
            {
                Debug.Log($"Max {selectedTower.towerName} reached!");
                selectedTower = null;
                return;
            }
            if (ResourceManager.Instance == null)
            {
                Debug.LogError("ResourceManager.Instance is null!");
                selectedTower = null;
                return;
            }
            if (!ResourceManager.Instance.CanAfford(selectedTower.baseCost))
            {
                Debug.Log("Not enough resources!");
                selectedTower = null;
                return;
            }
            Debug.Log($"Selecting {selectedTower.towerName}, creating preview...");
            CreatePreview();
            if (towerPreview != null)
            {
                Vector3 initialPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                initialPos.z = 0;
                initialPos.x = Mathf.Round(initialPos.x);
                initialPos.y = Mathf.Round(initialPos.y);
                towerPreview.transform.position = initialPos;
            }
        }
    }

    private void CreatePreview()
    {
        if (towerPreview != null)
        {
            Destroy(towerPreview);
        }

        towerPreview = new GameObject("TowerPreview");
        SpriteRenderer sr = towerPreview.AddComponent<SpriteRenderer>();
        if (selectedTower.sprite != null)
        {
            sr.sprite = selectedTower.sprite;
        }
        else
        {
            Debug.LogError("selectedTower.sprite is null!");
        }
        sr.color = new Color(1, 1, 1, 0.5f);
        sr.sortingLayerName = "Items";
        sr.sortingOrder = 1;
        Debug.Log("Preview created with sprite: " + sr.sprite?.name);
    }

    private void HandleBuildingPlacement()
    {
        if (towerPreview == null)
        {
            Debug.LogError("towerPreview is null in HandleBuildingPlacement! Recreating...");
            CreatePreview();
            if (towerPreview == null) return;
        }

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        mousePos.x = Mathf.Round(mousePos.x);
        mousePos.y = Mathf.Round(mousePos.y);
        towerPreview.transform.position = mousePos;

        bool canPlace = !Physics2D.OverlapCircle(mousePos, 0.8f);
        SpriteRenderer sr = towerPreview.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = canPlace ? new Color(1, 1, 1, 0.5f) : new Color(1, 0, 0, 0.5f);
        }

        // Place with left-click or hold
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && canPlace)
        {
            if (towerCounts[selectedTower.towerName] < selectedTower.maxQuantity &&
                ResourceManager.Instance.CanAfford(selectedTower.baseCost))
            {
                PlaceBuilding(mousePos);
            }
            else if (towerCounts[selectedTower.towerName] >= selectedTower.maxQuantity)
            {
                Debug.Log($"Max {selectedTower.towerName} reached!");
                CancelPlacement();
            }
            else
            {
                Debug.Log("Not enough resources to place another!");
                CancelPlacement();
            }
        }
        // Cancel with right-click
        else if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }

    private void PlaceBuilding(Vector3 position)
    {
        int index = System.Array.IndexOf(availableTowers, selectedTower);
        GameObject building = Instantiate(towerPrefabs[index], position, Quaternion.identity);
        building.GetComponent<Tower>().data = selectedTower;
        towerCounts[selectedTower.towerName]++;
        ResourceManager.Instance.SpendResources(selectedTower.baseCost);
        // Recreate preview instead of destroying it
        CreatePreview();
        towerPreview.transform.position = position; // Keep preview at last placement
    }

    private void CancelPlacement()
    {
        Destroy(towerPreview);
        towerPreview = null;
        selectedTower = null;
    }
}