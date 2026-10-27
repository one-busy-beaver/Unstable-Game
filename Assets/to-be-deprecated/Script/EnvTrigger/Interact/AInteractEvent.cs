using UnityEngine;

public abstract class InteractEvent: MonoBehaviour
{
    [SerializeField] protected bool autoTrigger = false; 
    [SerializeField] private bool fill = true;
    [Header("Visuals (Editor Only)")]
    [SerializeField] private Color gizmoColor = new Color(0.65f, 0.89f, 0.34f, 0.3f); // #a6e356 with alpha

    // Add this so child classes can define what happens on interaction
    public abstract void Execute();

    // ================================ Visualization ================================

    private void OnDrawGizmos()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box == null) return;

        // Capture current matrix and apply the object's transform
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = gizmoColor;
        if (fill) Gizmos.DrawCube(box.offset, box.size);

        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1.0f);
        Gizmos.DrawWireCube(box.offset, box.size);

        // Reset matrix so other gizmos aren't affected
        Gizmos.matrix = oldMatrix;
    }
}