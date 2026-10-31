using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerState))]
public class Player : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] InputActionReference moveAction;
    [SerializeField] InputActionReference jumpAction;

    [Header("Walking Settings")]
    [SerializeField] float walkSpeed = 8f;

    [Header("Jump Settings")]
    [SerializeField] float jumpSpeed = 10f;
    [SerializeField] float jumpBufferTime = 0.1f;
    [SerializeField] float coyoteTime = 0.1f;
    [SerializeField] float fallMultiplier = 2f;
    [SerializeField] float riseMultiplier = 8f;

    [Header("Gravity Settings")]
    [SerializeField] float gravity = -30f;
    [SerializeField] float maxFallSpeed = 30f;

    [Header("Collision Settings")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] float maxSlopeAngle = 50f;
    [SerializeField] float groundSnapDistance = 0.3f; // How far down we stick to the ground when walking downhill

    const float Skin = 0.01f; // Small gap between player and colliders

    // Player components
    Rigidbody2D rb;
    Collider2D bodyCollider;
    PlayerState state;

    // Private variables
    float moveInputX;
    float velocityY;
    float jumpBufferTimer;
    float coyoteTimer;
    bool jumpHeld;
    ContactFilter2D groundFilter;
    readonly RaycastHit2D[] hits = new RaycastHit2D[8];
    readonly Collider2D[] overlaps = new Collider2D[8];

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        state = GetComponent<PlayerState>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        groundFilter.SetLayerMask(groundMask);
        groundFilter.useTriggers = false;
    }

    void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
    }

    void Update()
    {
        moveInputX = moveAction.action.ReadValue<Vector2>().x;

        if (moveInputX < 0) state.facingLeft = true;
        else if (moveInputX > 0) state.facingLeft = false;

        if (jumpAction.action.WasPressedThisFrame()) jumpBufferTimer = jumpBufferTime;
        jumpHeld = jumpAction.action.IsPressed();
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        //ResolveOverlaps();

        // JUMP
        if (state.grounded) coyoteTimer = coyoteTime;
        else coyoteTimer -= dt;

        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            velocityY = jumpSpeed;
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }
        jumpBufferTimer -= dt;

        float gravityScale = 1f;
        if (velocityY < 0f) gravityScale = fallMultiplier;
        else if (velocityY > 0f && !jumpHeld) gravityScale = riseMultiplier;
        velocityY = Mathf.Max(velocityY + gravity * gravityScale * dt, -maxFallSpeed);

        // WALK
        Move(new Vector2(moveInputX * walkSpeed * dt, 0f), climbSlopes: true);
        bool hitVertical = Move(new Vector2(0f, velocityY * dt));

        // COLLISION
        bool wasGrounded = state.grounded;
        // Only a hit while falling means floor; a hit while rising is a ceiling.
        // Being held up by an edge or corner doesn't count: there must be ground under our middle.
        state.grounded = hitVertical && velocityY < 0f && HasGroundBelow();
        if (hitVertical) velocityY = 0f;

        // Walking downhill leaves us floating a little each step; stick back onto the ground
        if (wasGrounded && !state.grounded && velocityY <= 0f && SnapToGround())
        {
            state.grounded = HasGroundBelow();
            velocityY = 0f;
        }
    }

    // Moves by delta, but stops at the first collider in the way. Returns true if something was hit.
    // With climbSlopes, running into a walkable slope moves us up along it instead of stopping.
    bool Move(Vector2 delta, bool climbSlopes = false)
    {
        float distance = delta.magnitude;
        if (distance == 0f) return false;
        Vector2 direction = delta / distance;

        if (!CastClosest(direction, distance, out RaycastHit2D hit))
        {
            rb.position += delta;
            return false;
        }

        // Go only as far as the closest hit, minus the skin gap
        // Hitting a surface at an angle needs a bigger step back to keep the full skin gap from it
        float skinAlongMove = Skin / Mathf.Max(-Vector2.Dot(direction, hit.normal), 0.1f);
        float travel = Mathf.Clamp(hit.distance - skinAlongMove, 0f, distance);
        rb.position += direction * travel;

        // Walked into a slope: spend the rest of the move going up along its surface
        if (climbSlopes && IsWalkable(hit.normal))
        {
            Vector2 remaining = direction * (distance - travel);
            Move(remaining - Vector2.Dot(remaining, hit.normal) * hit.normal);
        }
        return true;
    }

    // "If I slid my collider this way, what would I hit first?" Returns false if nothing is in the way.
    bool CastClosest(Vector2 direction, float distance, out RaycastHit2D closest)
    {
        closest = default;
        int count = rb.Cast(direction, groundFilter, hits, distance + Skin);
        for (int i = 0; i < count; i++)
        {
            if (i == 0 || hits[i].distance < closest.distance) closest = hits[i];
        }
        return count > 0;
    }

    // If there's walkable ground a short way below, move down onto it. Returns true if we snapped.
    bool SnapToGround()
    {
        if (!CastClosest(Vector2.down, groundSnapDistance, out RaycastHit2D hit)) return false;
        if (!IsWalkable(hit.normal)) return false;

        rb.position += Vector2.down * Mathf.Max(0f, hit.distance - Skin);
        return true;
    }

    // Is there walkable ground right under the middle of our feet?
    // A ray hits the real face of the ground, so an edge or corner touching the side of our round bottom can't fool it.
    bool HasGroundBelow()
    {
        Bounds bounds = bodyCollider.bounds;
        // Start a little inside our collider, so the ray never begins below the ground
        Vector2 origin = new(bounds.center.x, bounds.min.y + 0.1f);

        // Standing on the steepest walkable slope, the ground under our middle is this far below our lowest point
        float radius = bounds.extents.x;
        float slopeDrop = radius * (1f / Mathf.Cos(maxSlopeAngle * Mathf.Deg2Rad) - 1f);
        float reach = 0.1f + slopeDrop + 2f * Skin;

        int count = Physics2D.Raycast(origin, Vector2.down, groundFilter, hits, reach);
        for (int i = 0; i < count; i++)
        {
            if (hits[i].rigidbody == rb) continue; // Our own collider
            return IsWalkable(hits[i].normal);
        }
        return false;
    }

    // Casts can't see out of a collider we're already inside, so every Move would be blocked.
    // Push ourselves out (plus the skin gap) before moving.
    void ResolveOverlaps()
    {
        int count = rb.Overlap(groundFilter, overlaps);
        for (int i = 0; i < count; i++)
        {
            ColliderDistance2D d = overlaps[i].Distance(bodyCollider);
            if (!d.isOverlapped) continue;

            Vector2 push = d.pointA - d.pointB;
            // Inside the ground: get out straight up, not sideways, or standing on a slope slowly slides us down it
            if (IsWalkable(push.normalized)) push = Vector2.up * (push.sqrMagnitude / push.y);
            rb.position += push + push.normalized * Skin;
            Debug.Log($"Pushed out of {overlaps[i].name} by {push.magnitude:F3}");
        }
    }

    // Is this surface flat enough to walk on? (normal = the direction the surface faces)
    bool IsWalkable(Vector2 normal)
    {
        return Vector2.Angle(normal, Vector2.up) <= maxSlopeAngle;
    }
}
