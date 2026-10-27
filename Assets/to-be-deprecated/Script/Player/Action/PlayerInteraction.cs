using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private InteractHandler _currentInteract;

    private void Update()
    {
        bool interactPressed = InputManager.Instance.Controls.Player.Interact.triggered;

        if (_currentInteract != null && interactPressed)
        {
            _currentInteract.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Use TryGetComponent for better performance and cleaner code
        if (other.TryGetComponent(out InteractHandler interactable))
        {
            _currentInteract = interactable;
            _currentInteract.TogglePrompt(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out InteractHandler interactable) && interactable == _currentInteract)
        {
            _currentInteract.TogglePrompt(false);
            _currentInteract = null;
        }
    }
}