using System.Collections;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashTime = 0.15f;
    [SerializeField] private float dashCooldown = 0.3f;
    [SerializeField] private GameObject dashEffect;

    // Player components
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerStates pState;
    
    // Private variables
    private bool canDash = true;
    private bool dashed = false;
    private bool dashPressed;
    private float originalGravity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        pState = GetComponent<PlayerStates>();
        
        originalGravity = rb.gravityScale;
    }

    void Update()
    {
        dashPressed = InputManager.Instance.Controls.Player.Dash.triggered;
        HandleDash();
    }

    private void HandleDash()
    {
        // GATEKEEPER: Check if ability is unlocked
        if (PlayerInventory.Instance == null || !PlayerInventory.Instance.HasDash) return;

        if (dashPressed && canDash && !dashed)
        {
            StartCoroutine(DashRoutine());
            dashed = true;
        }
        
        if (pState.onGround)
        {
            dashed = false;
        }
    }

    private IEnumerator DashRoutine()
    {
        canDash = false;
        pState.isDashing = true;
        anim.SetTrigger("Dashing");
        rb.gravityScale = 0;

        // Dash at the direction the character is facing
        rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0); 

        if (pState.onGround)
        {
            // Instanstiate dash effect as a child object
            Instantiate(dashEffect, transform); 
        }

        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = originalGravity;
        pState.isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }
}