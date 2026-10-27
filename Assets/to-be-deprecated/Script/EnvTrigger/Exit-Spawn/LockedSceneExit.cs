using UnityEngine;

public class LockedSceneExit : SceneExit
{
    [Header("Lock Settings")]
    [SerializeField] EventID unlockFlag; 
    [SerializeField] string lockedMessage = "Locked";
    [SerializeField] string unlockAvailableMessage = "Unlock";

    InteractHandler handler;

    void Start()
    {
        handler = GetComponent<InteractHandler>();
        RefreshDoorUI();
    }

    // Update the UI as soon as the player touches the trigger zone
    new void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RefreshDoorUI();
        }
        
        // Base logic handles the auto-trigger transition
        base.OnTriggerEnter2D(other);
    }

    void RefreshDoorUI()
    {
        bool isUnlocked = WorldState.Instance.GetFlag(unlockFlag);

        // Condition 1: If already unlocked, use default UI (No change)
        if (isUnlocked) 
        {
            return; 
        }

        // Condition 2 & 3: Check inventory if still locked
        if (PlayerInventory.Instance.HasBaseKey)
        {
            // Player has key: Show "Unlock"
            handler.SetPromptText(unlockAvailableMessage);
        }
        else
        {
            // Player lacks key: Show "Locked"
            handler.SetPromptText(lockedMessage);
        }
    }

    public override void Execute()
    {
        bool isUnlocked = WorldState.Instance != null && WorldState.Instance.GetFlag(unlockFlag);

        // STEP 2: If the door is already unlocked, perform the transition
        if (isUnlocked)
        {
            base.Execute();
            return;
        }

        // STEP 1: If the door is locked, try to unlock it
        if (PlayerInventory.Instance != null && PlayerInventory.Instance.HasBaseKey)
        {
            // Unlock the door in WorldState
            WorldState.Instance.SetFlag(unlockFlag, true); 
            
            // Refresh UI to change text from "Unlock" back to your default text
            RefreshDoorUI();

            // Forces the UI to redraw the new text
            handler.TogglePrompt(false); 
            handler.TogglePrompt(true); 
            
            // Optional: You can re-enable autoTrigger here if you want it to 
            // trigger automatically NEXT time the player walks into it.
            // autoTrigger = true;

            Debug.Log("Door Unlocked! Interact again to enter.");
        }
        else
        {
            // Feedback if player lacks the key
            if (handler != null) handler.TogglePrompt(true);
        }
    }
}