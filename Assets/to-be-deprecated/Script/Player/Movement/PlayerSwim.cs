using UnityEngine;

public class PlayerSwim : MonoBehaviour
{
    [Header("Swimming Settings")]
    [SerializeField] float swimSinkSpeed = 4f;
    [SerializeField] float swimUpForce = 7f;
    [SerializeField] float swimSubmergeTime = 7f;

    [Header("Drowning Settings")]
    [SerializeField] float drownSinkSpeed = 2f;
    [SerializeField] float noSwimUpForce = 0f;
    [SerializeField] float drownSubmergeTime = 0.5f;
    [SerializeField] int drownDamage = 1;

    [Header("Active Config")]
    [SerializeField] float activeSinkSpeed;
    [SerializeField] float activeSwimUpForce;
    [SerializeField] float activeMaxSubmergeTime;
    
    // Player components
    Rigidbody2D rb;
    PlayerStates pState;
    LastSafeGround tracker;
    PlayerHealth health;

    // Private variables
    float currentSubmergeTime;    
    float originalGravity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pState = GetComponent<PlayerStates>();
        tracker = GetComponent<LastSafeGround>();
        health = GetComponent<PlayerHealth>();
        originalGravity = rb.gravityScale;
    }

    void Update()
    {
        if (!pState.inWater) 
        {
            currentSubmergeTime = 0f;
            return;
        }

        if (!pState.onGround)
        {
            HandleSwim();
        }

        if (pState.isSubmerged) 
            HandleDrown();
        else
            currentSubmergeTime = 0f;
    }

    private void HandleSwim()
    {
        UpdateActiveAbilities();

        // Use your existing InputManager
        if (InputManager.Instance.Controls.Player.Jump.triggered)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, activeSwimUpForce);
        }
        else if (rb.linearVelocity.y < -activeSinkSpeed)
        {
            // Clamp downward velocity so you sink slowly instead of dropping like a rock
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -activeSinkSpeed);
        }
    }

    public void UpdateActiveAbilities()
    {
        bool hasSwim = PlayerInventory.Instance != null && PlayerInventory.Instance.HasSwim;

        activeSinkSpeed = hasSwim ? swimSinkSpeed : drownSinkSpeed;
        activeSwimUpForce = hasSwim ? swimUpForce : noSwimUpForce;
        activeMaxSubmergeTime = hasSwim ? swimSubmergeTime : drownSubmergeTime;
    }

    private void HandleDrown()
    {
        currentSubmergeTime += Time.deltaTime;

        if (currentSubmergeTime >= activeMaxSubmergeTime)
        {
            DrownAndRespawn();
        }
    }

    public void EnterWater()
    {
        pState.inWater = true;
        currentSubmergeTime = 0f;
        
        // Dampen entry velocity and lower gravity so water feels heavy
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f); 
        rb.gravityScale = originalGravity * 0.5f; 
    }

    public void ExitWater()
    {
        pState.inWater = false;
        currentSubmergeTime = 0f;
        rb.gravityScale = originalGravity; // Restore normal gravity
    }

    private void DrownAndRespawn()
    {   
        health.TakeDamage(drownDamage);

        if (tracker != null)
        {
            transform.position = tracker.GetLastSafePosition();
        }
        else
        {
            Debug.LogWarning("LastSafeGround missing!");
        }
        
        rb.linearVelocity = Vector2.zero;
        ExitWater();
    }
}