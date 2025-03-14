using UnityEngine;

public class Center : Building
{
    private float currentRange;
    private GameObject rangeVisual;
    private LineRenderer rangeBorder;
    private SpriteRenderer rangeFill;

    protected override void Start()
    {
        base.Start();
        GameManager.Instance.SetCenter(this);
        InitializeRangeVisual();
    }

    protected override void UpdateStats()
    {
        base.UpdateStats();
        CenterData centerData = (CenterData)data;
        if (centerData.rangeMultipliers == null || centerData.rangeMultipliers.Length < currentLevel)
        {
            Debug.LogError($"{data.buildingName} has invalid rangeMultipliers array! Expected size >= {currentLevel}, got {centerData.rangeMultipliers?.Length ?? 0}");
            currentRange = centerData.baseRange;
            return;
        }
        currentRange = centerData.baseRange * centerData.rangeMultipliers[currentLevel - 1];
        UpdateRangeVisual();
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        if (currentHealth <= 0) GameManager.Instance.GameOver();
    }

    private void InitializeRangeVisual()
    {
        // Create parent object
        rangeVisual = new GameObject("RangeVisual");
        rangeVisual.transform.SetParent(transform);
        rangeVisual.transform.localPosition = Vector3.zero;
        rangeVisual.SetActive(false);

        // LineRenderer for border
        rangeBorder = rangeVisual.AddComponent<LineRenderer>();
        rangeBorder.positionCount = 5;
        rangeBorder.startWidth = 0.1f;
        rangeBorder.endWidth = 0.1f;
        rangeBorder.material = new Material(Shader.Find("Sprites/Default"));
        rangeBorder.startColor = Color.white;
        rangeBorder.endColor = Color.white;
        rangeBorder.sortingLayerName = "Items";
        rangeBorder.sortingOrder = 2;

        // Create fill with SpriteRenderer and dynamic sprite
        GameObject fillObject = new GameObject("RangeFill");
        fillObject.transform.SetParent(rangeVisual.transform);
        fillObject.transform.localPosition = Vector3.zero;
        rangeFill = fillObject.AddComponent<SpriteRenderer>();

        // Create a 1x1 white sprite dynamically
        Texture2D fillTexture = new Texture2D(1, 1);
        fillTexture.SetPixel(0, 0, Color.white);
        fillTexture.Apply();
        rangeFill.sprite = Sprite.Create(fillTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 100f); // Pivot at center, 100 pixels per unit
        rangeFill.color = new Color(1, 1, 1, 0.2f);
        rangeFill.sortingLayerName = "Items";
        rangeFill.sortingOrder = 1;

        UpdateRangeVisual();
    }

    private void UpdateRangeVisual()
    {
        if (rangeVisual == null) return;

        float size = currentRange * 2f;
        rangeFill.transform.localScale = new Vector3(size, size, 1f);

        Vector3 halfSize = new Vector3(currentRange, currentRange, 0);
        rangeBorder.SetPosition(0, transform.position - halfSize);
        rangeBorder.SetPosition(1, transform.position + new Vector3(-currentRange, currentRange, 0));
        rangeBorder.SetPosition(2, transform.position + halfSize);
        rangeBorder.SetPosition(3, transform.position + new Vector3(currentRange, -currentRange, 0));
        rangeBorder.SetPosition(4, transform.position - halfSize);
    }

    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ToggleRangeVisual();
        }
    }

    public void ToggleRangeVisual()
    {
        if (rangeVisual != null)
        {
            rangeVisual.SetActive(!rangeVisual.activeSelf);
            Debug.Log($"{data.buildingName} range visibility toggled: {rangeVisual.activeSelf}");
        }
    }

    public void HideRangeVisual()
    {
        if (rangeVisual != null && rangeVisual.activeSelf)
        {
            rangeVisual.SetActive(false);
            Debug.Log($"{data.buildingName} range hidden.");
        }
    }

    void OnDestroy()
    {
        if (rangeVisual != null)
        {
            Destroy(rangeVisual);
        }
    }
}