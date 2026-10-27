using UnityEngine;

public class PlayerWalk : MonoBehaviour
{
    [Header("Walking Settings")]
    [SerializeField] float walkSpeed = 8f;
    
    // Player components
    Rigidbody2D rb;
    Animator anim;
    PlayerStates pState;

    // Private variables
    float moveInputX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        pState = GetComponent<PlayerStates>();
    }

    void Update()
    {
        // Exit early if the game is paused
        if (Time.timeScale == 0) return;
    
        moveInputX = InputManager.Instance.Controls.Player.Move.ReadValue<Vector2>().x;

        if (!pState.isDashing)
        {
            HandleTurn();
            if (!pState.isClimbing) HandleWalk();
        }
    }

    void HandleTurn()
    {
        if (moveInputX < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
        else if (moveInputX > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
    }

    void HandleWalk()
    {
        rb.linearVelocity = new Vector2(walkSpeed * moveInputX, rb.linearVelocity.y);
        anim.SetBool("Walking", rb.linearVelocity.x != 0 && pState.onGround);
    }
}