using UnityEngine;


public class ItemPickup : InteractEvent
{
    [Header("Ability Settings")]
    [SerializeField] private CollectID uniqueID;
    [SerializeField] private bool destroyOnPickup = true;

    void Awake() {
        if (WorldState.Instance.IsCollected(uniqueID)) {
            Destroy(gameObject);
        }
    }

    public CollectID ID => uniqueID;

    public override void Execute()
    {
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.Unlock(uniqueID);
            
            Debug.Log($"Unlocked ability: {uniqueID}");

            if (destroyOnPickup)
            {
                OnPickUp();
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.LogError("PlayerInventory instance not found in the scene!");
        }
    }

    public void OnPickUp() {
        WorldState.Instance.MarkAsCollected(uniqueID);
        Destroy(gameObject);
    }
}