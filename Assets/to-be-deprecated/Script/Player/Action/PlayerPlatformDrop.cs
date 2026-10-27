using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPlatformDrop : MonoBehaviour
{
    private OneWayPlatform _currentPlatform;

    private void OnEnable() 
    {
        // Replace 'Player.Move' with your specific Map and Action names
        InputManager.Instance.Controls.Player.Move.performed += HandleInput;
    }

    private void OnDisable() 
    {
        if (InputManager.Instance?.Controls != null)
            InputManager.Instance.Controls.Player.Move.performed -= HandleInput;
    }

    private void HandleInput(InputAction.CallbackContext context) 
    {
        if (_currentPlatform != null && context.ReadValue<Vector2>().y < -0.5f) 
        {
            _currentPlatform.OnDrop();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out OneWayPlatform platform))
        {
            _currentPlatform = platform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out OneWayPlatform platform))
        {
            _currentPlatform = null;
        }
    }
}