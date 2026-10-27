using UnityEngine;

public class EventTracker: InteractEvent 
{
    [SerializeField] EventID eventName;

    public override void Execute() 
    {
        // Toggle the current state
        bool currentState = WorldState.Instance.GetFlag(eventName);
        WorldState.Instance.SetFlag(eventName, !currentState);
    }
}