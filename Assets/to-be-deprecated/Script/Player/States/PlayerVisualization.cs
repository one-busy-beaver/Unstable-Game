using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualization : MonoBehaviour
{
    [Header("Visualization Settings")]
    [SerializeField] bool visualize = true;
    [SerializeField] private int maxPoints = 10;  // how long the trail lasts
    [SerializeField] private float recordInterval = 0.05f;

    // Player components
    private Rigidbody2D rb;
    private LineRenderer lineRenderer;

    // Private variables
    private float recordTimer;
    private List<Vector3> points = new List<Vector3>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        bool hasCourage = PlayerInventory.Instance != null && PlayerInventory.Instance.HasCourage;

    if (visualize || hasCourage)
        {
            DrawTrail();
        }
    }

    void DrawTrail()
    {
        recordTimer += Time.deltaTime;

        if (recordTimer >= recordInterval)
        {
            recordTimer = 0f;
            RecordPosition();
        }
    }
    
    void RecordPosition()
    {
        points.Add(rb.position);

        // Limit trail length
        if (points.Count > maxPoints) {
            points.RemoveAt(0);
        }

        // Update the line
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}