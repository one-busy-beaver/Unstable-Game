using UnityEngine;

public class LastSafeGround : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float updateInterval = 0.2f; // How often to save position

    private PlayerStates pState;
    private Vector3 lastSafePosition;
    private UniquePlayer player;
    private float timer;

    void Start()
    {
        player = GetComponent<UniquePlayer>();
        // Initialize with starting position
        lastSafePosition = transform.position;
        pState = GetComponent<PlayerStates>();

    }

    void Update()
    {
        timer += Time.deltaTime;

        // We only update the safe position if the player is grounded 
        // and a small interval has passed to avoid saving "cliff edge" frames.
        if (timer >= updateInterval)
        {
            if (player != null && pState.onGround && !pState.inWater)
            {
                lastSafePosition = transform.position;
            }
            timer = 0;
        }
    }

    public Vector3 GetLastSafePosition()
    {
        return lastSafePosition;
    }
}