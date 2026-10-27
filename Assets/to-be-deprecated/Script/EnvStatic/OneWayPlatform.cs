using UnityEngine;
using System.Collections;

public class OneWayPlatform : MonoBehaviour
{
    private PlatformEffector2D _effector;
    private bool _isPlayerOnPlatform;

    void Start()
    {
        _effector = GetComponent<PlatformEffector2D>();
    }

    // Triggered by your Input Action Asset (e.g., "Down" button)
    public void OnDrop()
    {
        if (_isPlayerOnPlatform)
        {
            StartCoroutine(DropRoutine());
        }
    }

    private IEnumerator DropRoutine()
    {
        // Flip the solid arc to the bottom so the player falls through
        _effector.rotationalOffset = 180f;
        yield return new WaitForSeconds(0.3f); // Wait long enough to clear the platform
        _effector.rotationalOffset = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) _isPlayerOnPlatform = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) _isPlayerOnPlatform = false;
    }
}