using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance;
    public Collider2D activeBound;

    // [Header("Zoom")]
    // [SerializeField] float zoomSpeed = 20f;
    // [SerializeField] float zoomOut = 5f;
    // [SerializeField] float zoomIn = 5f;

    [Header("Pan")]
    [SerializeField] float panSpeed = 8f;
    [SerializeField] float recenterSpeed = 8f;

    [Header("Player Bounds")]
    [SerializeField] float maxHorizontalOffset = 3f;
    [SerializeField] float maxVerticalOffset = 2f;

    private CinemachineVirtualCamera vcam;
    private CinemachineConfiner2D confiner;
    private CinemachineFramingTransposer transposer;
    // private float baseFOV;
    // private float zoomModifier = 0f;
    private Vector2 manualOffset;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        vcam = GetComponent<CinemachineVirtualCamera>();
        confiner = GetComponent<CinemachineConfiner2D>(); 
        transposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();

        // baseFOV = vcam.m_Lens.FieldOfView;
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void Update()
    {
        //HandleZoom();
        HandlePan();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "_Bootstrap") return;

        // Find the specific confiner in the newly loaded scene
        GameObject confinerObj = GameObject.FindWithTag("Confiner");
        
        if (confinerObj != null && confiner != null)
        {
            if (confinerObj.TryGetComponent<Collider2D>(out var bounds)) 
            {
                // Forces Cinemachine to recalculate the bounds for the new shape
                activeBound = bounds;
                confiner.m_BoundingShape2D = bounds;
                confiner.InvalidateCache(); 
            }

            Debug.Log($"CameraFollow: Found object with tag 'Confiner' in {scene.name}.");
        }
    }

    // Called directly by your SceneLoader to attach the newly spawned player
    public void SetTarget(GameObject newTarget)
    {
        if (vcam != null)
        {
            vcam.Follow = newTarget.transform;
            // vcam.LookAt = newTarget.transform; // Uncomment if your game is Top-Down/3D and requires LookAt
        }
    }

    // void HandleZoom()
    // {
    //     float zoomInput = InputManager.Instance.Controls.Camera.Zoom.ReadValue<float>();

    //     if (Mathf.Abs(zoomInput) > 0.01f)
    //     {
    //         zoomModifier += zoomInput * zoomSpeed * Time.deltaTime;
    //     }

    //     zoomModifier = Mathf.Clamp(zoomModifier, -zoomOut, zoomIn);

    //     float targetFOV = baseFOV - zoomModifier;

    //     vcam.m_Lens.FieldOfView = Mathf.Lerp(vcam.m_Lens.FieldOfView, targetFOV, Time.deltaTime * zoomSpeed * 10f);

    //     // Trick the Confiner2D by updating OrthographicSize
    //     // formula: OrthoSize = Distance * tan(FOV / 2)
    //     float distance = Mathf.Abs(vcam.transform.position.z); 
    //     float halfFOVRad = vcam.m_Lens.FieldOfView * 0.5f * Mathf.Deg2Rad;
    //     vcam.m_Lens.OrthographicSize = distance * Mathf.Tan(halfFOVRad);
    // }

    void HandlePan()
    {
        Vector2 input = InputManager.Instance.Controls.Camera.Pan.ReadValue<Vector2>();

        if (Mathf.Abs(input.x) > 0.01f)
        {
            manualOffset.x += input.x * panSpeed * Time.deltaTime;
        }
        else
        {
            manualOffset.x = Mathf.Lerp(manualOffset.x, 0, recenterSpeed * Time.deltaTime);
        }

        if (Mathf.Abs(input.y) > 0.01f)
        {
            manualOffset.y += input.y * panSpeed * Time.deltaTime;
        }
        else
        {
            manualOffset.y = Mathf.Lerp(manualOffset.y, 0, recenterSpeed * Time.deltaTime);
        }

        manualOffset.x = Mathf.Clamp(manualOffset.x, -maxHorizontalOffset, maxHorizontalOffset);
        manualOffset.y = Mathf.Clamp(manualOffset.y, -maxVerticalOffset, maxVerticalOffset);

        if (transposer != null)
        {
            transposer.m_TrackedObjectOffset = new Vector3(manualOffset.x, manualOffset.y, 0);
        }
    }
}
