using UnityEngine;

// Makes the sprite match the player's state. Never moves the player.
[RequireComponent(typeof(PlayerState), typeof(SpriteRenderer))]
public class PlayerVisuals : MonoBehaviour
{
    // Player components
    PlayerState state;
    SpriteRenderer body;

    void Awake()
    {
        state = GetComponent<PlayerState>();
        body = GetComponent<SpriteRenderer>();
    }

    // LateUpdate runs after every Update, so the state is already up to date for this frame
    void LateUpdate()
    {
        body.flipX = state.facingLeft;
    }
}
