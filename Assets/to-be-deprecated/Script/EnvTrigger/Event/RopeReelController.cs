using System.Collections;
using UnityEngine;

public class RopeReelController : MonoBehaviour
{
    [SerializeField] EventID eventName;
    [Tooltip("Rotation speed in degrees per second")]
    [SerializeField] float speed = 50f;
    [Tooltip("Z-axis for standard 2D CCW rotation")]
    [SerializeField] Vector3 rotationAxis = Vector3.forward; 
    [Tooltip("Total degrees to rotate")]
    [SerializeField] float rotationAmount = 270f;

    private Quaternion startRot;
    private float currentAngle = 0f;
    private Coroutine moveRoutine;

    void Awake() 
    {
        startRot = transform.rotation;
    }

    void Start() 
    {
        UpdateRopeReelPosition(WorldState.Instance.GetFlag(eventName));
    }

    void OnEnable() 
    {
        WorldState.OnStateChanged += HandleStateChange;
    }

    void OnDisable() 
    {
        WorldState.OnStateChanged -= HandleStateChange;
    }

    private void HandleStateChange(EventID name, bool value) 
    {
        if (name == eventName) {
            if (moveRoutine != null) StopCoroutine(moveRoutine);
            moveRoutine = StartCoroutine(RotateRopeReelRoutine(value));
        }
    }

    private IEnumerator RotateRopeReelRoutine(bool isLowered) 
    {
        float targetAngle = isLowered ? rotationAmount : 0f;

        while (!Mathf.Approximately(currentAngle, targetAngle)) 
        {
            currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, speed * Time.deltaTime);
            transform.rotation = startRot * Quaternion.AngleAxis(currentAngle, rotationAxis);
            yield return null;
        }
    }

    private void UpdateRopeReelPosition(bool isLowered) 
    {
        currentAngle = isLowered ? rotationAmount : 0f;
        transform.rotation = startRot * Quaternion.AngleAxis(currentAngle, rotationAxis);
    }
}