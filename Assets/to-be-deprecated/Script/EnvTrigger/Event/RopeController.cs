using System.Collections;
using UnityEngine;

public class RopeController : MonoBehaviour 
{
    [SerializeField] EventID eventName;
    [SerializeField] float difference = 10;
    [SerializeField] float speed = 2f;
    [Tooltip("Set to true for the interior rope if it moves UP when the flag is true.")]
    [SerializeField] bool invert;
    
    private Vector3 startPos;
    private Coroutine moveRoutine;


    void Awake() 
    {
        // Capture the initial position to use as a base reference
        startPos = transform.position;
    }

    void Start() 
    {
        // Check initial state when the scene loads
        UpdateRopePosition(WorldState.Instance.GetFlag(eventName));
    }

    void OnEnable() 
    {
        // Start listening for changes
        WorldState.OnStateChanged += HandleStateChange;
    }

    void OnDisable() 
    {
        // Stop listening if the object is destroyed/disabled to prevent errors
        WorldState.OnStateChanged -= HandleStateChange;
    }

    private void HandleStateChange(EventID name, bool value) 
    {
        if (name == eventName) {
            if (moveRoutine != null) StopCoroutine(moveRoutine);
            moveRoutine = StartCoroutine(MoveRopeRoutine(value));
        }
    }

    private float CalculateOffset(bool isLowered) 
    {
        if (!isLowered) return 0;
        return invert ? difference : -difference;
    }

    private IEnumerator MoveRopeRoutine(bool isLowered) 
    {
        float targetY = startPos.y + CalculateOffset(isLowered);
        Vector3 targetPos = new Vector3(startPos.x, targetY, startPos.z);

        // Move until the distance is negligible
        while (Vector3.Distance(transform.position, targetPos) > 0.001f) {
            transform.position = Vector3.MoveTowards(
                transform.position, 
                targetPos, 
                speed * Time.deltaTime
            );
            yield return null; // Wait for next frame
        }

        transform.position = targetPos; // Final snap for precision
        moveRoutine = null;
    }

    private void UpdateRopePosition(bool isLowered) 
    {
        float yOffset = 0;

        if (isLowered) {
            yOffset = invert ? difference : -difference;
        }

        // Apply only to the Y axis while keeping original X and Z
        transform.position = new Vector3(startPos.x, startPos.y + yOffset, startPos.z);
    }
}