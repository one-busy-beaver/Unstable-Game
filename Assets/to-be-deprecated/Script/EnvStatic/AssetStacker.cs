using UnityEngine;

[ExecuteAlways]
public class AssetStacker : MonoBehaviour
{
    [Header("Assets")]
    public GameObject topPrefab;
    public GameObject columnPrefab;

    [Header("Settings")]
    public float segmentHeight = 0.8f; // Vertical stack offset
    public float segmentWidth = 1.8f; // Horizontal stack offset
    public float topXOffset = 0f;
    public float topYOffset = 0f;

    [SerializeField] private int columnCount = 1; // Vertical stack
    [SerializeField] private int rowCount = 1;  // Horizontal stack

    // Encapsulate count to enforce rules
    public int Count
    {
        get => columnCount;
        set
        {
            columnCount = Mathf.Max(0, value); // Bridge: x >= 0
            Build();
        }
    }

    // Property for rowCount to trigger rebuild via code if needed
    public int RowCount
    {
        get => rowCount;
        set
        {
            rowCount = Mathf.Max(1, value);
            Build();
        }
    }

    private void OnValidate()
    {
        // Enforce constraints in Inspector
        rowCount = Mathf.Max(1, rowCount);
        columnCount = Mathf.Max(0, columnCount);
        
        // Delay call to avoid "SendMessage cannot be called during Awake, CheckConsistency, or OnValidate"
#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += Build;
#endif
    }

    public void Build()
    {
        if (this == null || columnPrefab == null) return;
        
#if UNITY_EDITOR
        // Fix: Prevent execution if this is a persistent asset template 
        // instead of an instance in the scene or prefab editor.
        if (UnityEditor.EditorUtility.IsPersistent(this)) return;
#endif

        // Clear existing children
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        // Horizontal Loop
        for (int r = 0; r < rowCount; r++)
        {
            float currentX = r * segmentWidth;
            float currentY = 0;

            // 1. Place Top piece per row
            if (topPrefab != null)
            {
                GameObject top = Instantiate(topPrefab, transform);
                top.transform.localPosition = new Vector3(currentX + topXOffset, topYOffset, 0);
                currentY -= segmentHeight;
            }

            // 2. Place Columns per row
            for (int c = 0; c < columnCount; c++)
            {
                GameObject col = Instantiate(columnPrefab, transform);
                col.transform.localPosition = new Vector3(currentX, currentY, 0);
                currentY -= segmentHeight;
            }
        }

        // Refresh Composite if present
        var composite = GetComponent<CompositeCollider2D>();
        if (composite != null) composite.GenerateGeometry();
    }
}