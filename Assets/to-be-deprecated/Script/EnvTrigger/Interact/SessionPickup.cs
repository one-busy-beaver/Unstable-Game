using UnityEngine;

public class SessionPickup : InteractEvent
{
    public enum PickupType { Health, Fireball }

    [Header("Session Settings")]
    [SerializeField] private SessionID sessionID; // The unique ID for this instance
    [SerializeField] private PickupType type;     // What this item gives
    [SerializeField] private int amount = 1;      // How much it gives

    void Awake()
    {
        // Check WorldState to see if this item was already taken in this session
        if (WorldState.Instance != null && WorldState.Instance.IsDead(sessionID))
        {
            // If already taken, remove it from the scene
            Destroy(gameObject);
        }
    }

    public SessionID ID => sessionID;

    public override void Execute()
    {
        if (PlayerInventory.Instance != null)
        {
            // Apply the resource to the player
            if (type == PickupType.Health)
            {
                PlayerInventory.Instance.UpdateHealth(amount);
            }
            else if (type == PickupType.Fireball)
            {
                PlayerInventory.Instance.UpdateFireballs(amount);
            }

            // Mark this specific item as "dead" in the current session
            WorldState.Instance.MarkAsDead(sessionID);
            
            Debug.Log($"Session item collected: {type} (+{amount})");
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("PlayerInventory instance not found!");
        }
    }
}