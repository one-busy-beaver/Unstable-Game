using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] private int health = 3;
    [SerializeField] private GameObject contents; // The pickup object hidden inside

    private bool isBroken = false;

    private void Awake()
    {
        // Automatically find the pickup type in the child object
        ItemPickup permanentItem = contents.GetComponent<ItemPickup>();
        SessionPickup sessionItem = contents.GetComponent<SessionPickup>();

        bool alreadyTaken = false;

        // 1. Check if it's a permanent item (Key/Ability)
        if (permanentItem != null)
        {
            alreadyTaken = WorldState.Instance.IsCollected(permanentItem.ID);
        }
        // 2. Check if it's a session item (Health/Ammo)
        else if (sessionItem != null)
        {
            alreadyTaken = WorldState.Instance.IsDead(sessionItem.ID);
        }

        // If the item inside is already gone, the box shouldn't exist
        if (alreadyTaken)
        {
            gameObject.SetActive(false);
            return;
        }
        
        // Ensure contents are hidden at the start
        contents.SetActive(false);
    }

    public void ObjectHit(int damage)
    {
        if (isBroken) return;

        health -= damage;

        if (health <= 0)
        {
            Break();
        }
    }

    private void Break()
    {
        isBroken = true;

        // Reveal the hidden pickup
        if (contents != null)
        {
            contents.SetActive(true);
        }

        // Disable the box visuals and collision so the player can walk through
        // but keep the object alive so the child 'contents' stay active
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        // Optional: Trigger a breaking particle effect here
        Debug.Log("Box broken!");
    }
}