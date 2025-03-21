using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour
{
    public GameObject[] buildingPrefabs;
    private BuildingData selectedBuilding = null;
    private GameObject buildingPreview = null;
    private Camera mainCamera;
    private Dictionary<string, int> buildingCounts = new Dictionary<string, int>();
    private Dictionary<BuildingData, int> prefabIndices = new Dictionary<BuildingData, int>();
    private Center lastClickedCenter = null; // Track the last clicked Center
    private Building lastClickedBuilding;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("No Main Camera found!");
            mainCamera = FindFirstObjectByType<Camera>();
        }

        // Initialize building counts and prefab indices
        for (int i = 0; i < buildingPrefabs.Length; i++)
        {
            Building buildingComponent = buildingPrefabs[i].GetComponent<Building>();
            if (buildingComponent == null || buildingComponent.data == null)
            {
                Debug.LogError($"Prefab at index {i} has no Building component or data!");
                continue;
            }
            BuildingData data = buildingComponent.data;
            buildingCounts[data.buildingName] = 0;
            prefabIndices[data] = i;
        }
    }

    void Update()
    {
        if (selectedBuilding != null && (GameManager.Instance.HasCenter || selectedBuilding is CenterData))
        {
            if (buildingPreview == null && buildingCounts[selectedBuilding.buildingName] < selectedBuilding.maxQuantity)
            {
                CreatePreview();
            }
            if (buildingPreview != null)
            {
                HandleBuildingPlacement();
            }
        }
        else if (Input.GetMouseButtonDown(0)) // Left-click outside placement mode
        {
            // Prioritize UI clicks over raycast
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return; // Skip raycast if clicking on a UI element
            }

            HandleOutsideClick();
        }
    }

    public void SelectBuilding(int buildingIndex)
    {
        if (buildingIndex >= 0 && buildingIndex < buildingPrefabs.Length)
        {
            Building buildingComponent = buildingPrefabs[buildingIndex].GetComponent<Building>();
            if (buildingComponent == null || buildingComponent.data == null)
            {
                Debug.LogError($"Prefab at index {buildingIndex} has no Building component or data!");
                return;
            }
            selectedBuilding = buildingComponent.data;
            if (selectedBuilding.sprites == null || selectedBuilding.sprites.Length == 0 || selectedBuilding.sprites[0] == null)
            {
                Debug.LogError($"BuildingData for {selectedBuilding.buildingName} has no sprite for Level 1!");
                selectedBuilding = null;
                return;
            }
            if (!GameManager.Instance.HasCenter && !(selectedBuilding is CenterData))
            {
                Debug.Log("Must build Center first!");
                selectedBuilding = null;
                return;
            }
            if (buildingCounts[selectedBuilding.buildingName] >= selectedBuilding.maxQuantity)
            {
                Debug.Log($"Max {selectedBuilding.buildingName} reached! Cannot select more.");
                selectedBuilding = null;
                return;
            }
            if (ResourceManager.Instance == null || !ResourceManager.Instance.CanAfford(selectedBuilding.baseCost))
            {
                Debug.Log("Not enough resources or ResourceManager missing!");
                selectedBuilding = null;
                return;
            }
            Debug.Log($"Selecting {selectedBuilding.buildingName} for placement.");
            CreatePreview();
            Vector3 initialPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            initialPos.z = 0;
            initialPos = SnapPosition(initialPos);
            buildingPreview.transform.position = initialPos;
        }
    }

    private void CreatePreview()
    {
        if (buildingPreview != null)
        {
            Destroy(buildingPreview);
        }

        if (buildingCounts[selectedBuilding.buildingName] >= selectedBuilding.maxQuantity)
        {
            Debug.Log($"Max {selectedBuilding.buildingName} reached! Preview not created.");
            CancelPlacement();
            return;
        }

        buildingPreview = new GameObject("BuildingPreview");
        SpriteRenderer sr = buildingPreview.AddComponent<SpriteRenderer>();
        sr.sprite = selectedBuilding.sprites[0]; // Use the Level 1 sprite for the preview
        sr.color = new Color(1, 1, 1, 0.5f);
        sr.sortingLayerName = "Items";
        sr.sortingOrder = 1;
    }

    private void HandleBuildingPlacement()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        mousePos = SnapPosition(mousePos);
        buildingPreview.transform.position = mousePos;

        float overlapRadius = selectedBuilding is BarrierData ? 0.4f : 0.8f;
        bool canPlace = !Physics2D.OverlapCircle(mousePos, overlapRadius);
        SpriteRenderer sr = buildingPreview.GetComponent<SpriteRenderer>();
        sr.color = canPlace ? new Color(1, 1, 1, 0.5f) : new Color(1, 0, 0, 0.5f);

        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && canPlace)
        {
            if (buildingCounts[selectedBuilding.buildingName] < selectedBuilding.maxQuantity &&
                ResourceManager.Instance.CanAfford(selectedBuilding.baseCost))
            {
                PlaceBuilding(mousePos);
            }
            else if (buildingCounts[selectedBuilding.buildingName] >= selectedBuilding.maxQuantity)
            {
                Debug.Log($"Max {selectedBuilding.buildingName} reached during placement!");
                CancelPlacement();
            }
            else
            {
                Debug.Log("Not enough resources to place another!");
                CancelPlacement();
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }

    private Vector3 SnapPosition(Vector3 position)
    {
        if (selectedBuilding is BarrierData)
        {
            float x = Mathf.Floor(position.x) + 0.5f;
            float y = Mathf.Floor(position.y) + 0.5f;
            return new Vector3(x, y, 0);
        }
        else
        {
            float x = Mathf.Round(position.x);
            float y = Mathf.Round(position.y);
            return new Vector3(x, y, 0);
        }
    }

    private void PlaceBuilding(Vector3 position)
    {
        int index = prefabIndices[selectedBuilding];
        GameObject building = Instantiate(buildingPrefabs[index], position, Quaternion.identity);
        buildingCounts[selectedBuilding.buildingName]++;
        ResourceManager.Instance.SpendResources(selectedBuilding.baseCost);

        // Track the Center if placed
        if (selectedBuilding is CenterData)
        {
            lastClickedCenter = building.GetComponent<Center>();
        }

        if (buildingCounts[selectedBuilding.buildingName] >= selectedBuilding.maxQuantity)
        {
            Debug.Log($"Max {selectedBuilding.buildingName} reached after placing!");
            CancelPlacement();
        }
        else
        {
            CreatePreview();
            buildingPreview.transform.position = position;
        }
    }

    // New method to decrement the building count when a building is sold
    public void OnBuildingSold(Building building)
    {
        if (building == null || building.data == null) return;

        string buildingName = building.data.buildingName;
        if (buildingCounts.ContainsKey(buildingName))
        {
            buildingCounts[buildingName]--;
            if (buildingCounts[buildingName] < 0)
            {
                buildingCounts[buildingName] = 0; // Prevent negative counts
            }
            Debug.Log($"Sold {buildingName}. New count: {buildingCounts[buildingName]}");
        }

        // If the sold building was the last clicked Center, reset the reference
        if (building is Center && building == lastClickedCenter)
        {
            lastClickedCenter = null;
        }
        // If the sold building was the last clicked building, reset the reference
        if (building == lastClickedBuilding)
        {
            lastClickedBuilding = null;
        }
    }

    private void HandleOutsideClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null)
        {
            Building building = hit.collider.GetComponent<Building>();
            if (building != null)
            {
                if (building is Center)
                {
                    lastClickedCenter = building as Center;
                }

                BuildingInfoPanel panel = FindObjectOfType<BuildingInfoPanel>();
                if (panel != null)
                {
                    // If the same building is clicked again, toggle the panel's visibility
                    if (building == lastClickedBuilding && panel.panel.activeSelf)
                    {
                        panel.HidePanel();
                        lastClickedBuilding = null; // Reset to allow showing the panel again on the next click
                    }
                    else
                    {
                        panel.ShowPanel(building);
                        lastClickedBuilding = building;
                    }
                }
                return;
            }
        }

        if (lastClickedCenter != null)
        {
            lastClickedCenter.HideRangeVisual();
        }
        BuildingInfoPanel panelInstance = FindObjectOfType<BuildingInfoPanel>();
        if (panelInstance != null && panelInstance.panel.activeSelf)
        {
            panelInstance.HidePanel();
        }
        lastClickedBuilding = null; // Reset when clicking outside
    }

    private void CancelPlacement()
    {
        Destroy(buildingPreview);
        buildingPreview = null;
        selectedBuilding = null;
    }
}