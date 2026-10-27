using System;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Header("Jumping Settings")]
    [SerializeField] private float jumpForce = 11f;
    [SerializeField] private float jumpBufferTime = 1f;
    [SerializeField] private float coyoteTime = 0.1f; 
    [SerializeField] private int maxAirJumps = 0;
    [SerializeField] private float maxFallSpeed = 15f;
    [SerializeField] private float fallMultiplier = 3f; // faster fall
    [SerializeField] private float riseMultiplier = 8f; // slower rise if jump released early
    
    // Player components
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerStates pState;

    // Private variables
    private int airJumpCounter = 0;
    private float jumpBufferCounter = 1f; // extend jump register before touching the ground
    private float coyoteTimeCounter = 0.1f; // extend jump register after leaving the ground
    
    private bool jumpPressed; // For the initial jump
    private bool jumpHeld; // For variable jump height

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        pState = GetComponent<PlayerStates>();
    }

    void Update()
    {
        if (pState.isDashing) return;
        
        jumpPressed = InputManager.Instance.Controls.Player.Jump.triggered; 
        jumpHeld = InputManager.Instance.Controls.Player.Jump.IsPressed();

        UpdateJumpVariables();

        anim.SetBool("Jumping", !pState.onGround);
        
        if ((!pState.inWater || pState.onGround) && !pState.isClimbing) 
        {
            HandleJump();
        }
    }

    void FixedUpdate()
    {
        if (!pState.inWater && !pState.isClimbing) 
        {
            UpdateAirBorneState();
        }
    }

    void HandleJump()
    {
        if (!pState.isJumping)
        {
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                // Jump
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                pState.isJumping = true;
                jumpBufferCounter = 0f;
            }
            else if (!pState.onGround && airJumpCounter < maxAirJumps && jumpPressed)
            {
                // Air jump
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                pState.isJumping = true;
                airJumpCounter++;
                jumpBufferCounter = 0f;
            }
        }

        if (!jumpHeld && rb.linearVelocity.y > 0)
        {
            // Cancel jump (same line of code for released jump)
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (riseMultiplier - 1) * Time.fixedDeltaTime;

            pState.isJumping = false;
        }
    }

    void UpdateJumpVariables()
    {
        if (PlayerInventory.Instance != null)
        {
            maxAirJumps = PlayerInventory.Instance.HasDoubleJump ? 1 : 0;
        }

        if (pState.onGround || pState.canClimb)
        {
            pState.isJumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (jumpPressed)
        {
            // set jump buffer to the max
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            // Decrease jump buffer by one every frame
            jumpBufferCounter -= Time.deltaTime * 10;
        }
    }

    void UpdateAirBorneState()
    {
        if (rb.linearVelocity.y < 0)
        {
            if (Math.Abs(rb.linearVelocity.y) < maxFallSpeed)
            {
                // Falling: increase gravity to fall faster
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            }
            else
            {
                // Falling: reached the max fall speedanim
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxFallSpeed);
            }
        }
        else if (rb.linearVelocity.y > 0 && !jumpHeld)
        {
            // Released jump: slower rise
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (riseMultiplier - 1) * Time.fixedDeltaTime;
        }
    }
}