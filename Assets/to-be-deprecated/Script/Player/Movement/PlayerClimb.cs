using UnityEngine;

public class PlayerClimb : MonoBehaviour
{
    [Header("Climbing Settings")]
    [SerializeField] float climbSpeed = 6f;
    [SerializeField] Vector2 jumpOffForce = new Vector2(10f, 10f);

    // Player components
    Rigidbody2D rb;
    Animator anim;
    PlayerStates pState;

    // Private variables
    Vector2 moveInput;
    bool jumpPressed;
    float originalGravity;
    Transform currentRope; // Stores the rope to snap to

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        pState = GetComponent<PlayerStates>();
        originalGravity = rb.gravityScale;
    }

    void Update()
    {
        moveInput = InputManager.Instance.Controls.Player.Move.ReadValue<Vector2>();
        jumpPressed = InputManager.Instance.Controls.Player.Jump.triggered;

        HandleClimb();
    }

    void HandleClimb()
    {
        // Attach to rope (Ignore unless pressing UP)
        if (pState.canClimb && !pState.isClimbing && moveInput.y > 0.5f)
        {
            pState.isClimbing = true;
            rb.linearVelocity = Vector2.zero; // Kill horizontal momentum
            rb.gravityScale = 0f; // Disable gravity so player doesn't slide down
            
            // Snap player X position to the rope's center
            if (currentRope != null)
            {
                transform.position = new Vector3(currentRope.position.x, transform.position.y, transform.position.z);
            }
        }

        // Handle Active Climbing
        if (pState.isClimbing)
        {
            // Climb up and down
            rb.linearVelocity = new Vector2(0, moveInput.y * climbSpeed);

            anim.SetBool("Climbing", true);
            float animPlaySpeed = Mathf.Abs(moveInput.y) > 0.1f ? 1f : 0f;
            anim.SetFloat("climbSpeed", animPlaySpeed);

            // Jump off the rope
            if (jumpPressed)
            {
                DetachFromRope();
                
                // Jump in the direction the character is currently facing
                float faceDirection = transform.localScale.x;
                rb.linearVelocity = new Vector2(faceDirection * jumpOffForce.x, jumpOffForce.y);
            }

            // Naturally fall off if player climbs past the top or bottom of the rope
            if (!pState.canClimb || pState.onGround)
            {
                DetachFromRope();
            }
        }
    }

    void DetachFromRope()
    {
        pState.isClimbing = false;
        anim.SetBool("Climbing", false);
        rb.gravityScale = originalGravity; // Restore gravity
    }

    public void EnterRope(Transform ropeTransform)
    {
        pState.canClimb = true;
        currentRope = ropeTransform;
    }

    public void LeaveRope()
    {
        pState.canClimb = false;
        currentRope = null;
    }
}