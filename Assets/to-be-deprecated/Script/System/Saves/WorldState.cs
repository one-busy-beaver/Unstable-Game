using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{
    public static WorldState Instance;
    private HashSet<CollectID> collectedItems = new HashSet<CollectID>();
    private HashSet<EventID> worldEvent = new HashSet<EventID>();

    // Session: Enemies and Health/Fireball pickups (Resets on death)
    private HashSet<SessionID> sessionList = new HashSet<SessionID>();

    // An event that other scripts can subscribe to
    public static event Action<EventID, bool> OnStateChanged;

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    // --- Permanent Logic ---
    public void MarkAsCollected(CollectID id) => collectedItems.Add(id);
    public bool IsCollected(CollectID id) => collectedItems.Contains(id);

    // --- Session Logic (Enemies & Resources) ---
    public void MarkAsDead(SessionID uniqueID) => sessionList.Add(uniqueID);
    public bool IsDead(SessionID uniqueID) => sessionList.Contains(uniqueID);

    // Call this when the player dies and goes back to Main Menu
    public void ResetSession()
    {
        sessionList.Clear();
        Debug.Log("WorldState: Session reset. Enemies and resources will respawn.");
    }

    public void SetFlag(EventID eventName, bool value) {
        if (value) worldEvent.Add(eventName);
        else worldEvent.Remove(eventName);

        // Notify anyone listening that a state changed
        OnStateChanged?.Invoke(eventName, value);
    }

    public bool GetFlag(EventID eventName) => worldEvent.Contains(eventName);
}