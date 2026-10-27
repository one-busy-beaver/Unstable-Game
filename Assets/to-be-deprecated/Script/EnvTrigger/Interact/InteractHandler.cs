using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class InteractHandler : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] string promptText = "Interact"; 
    [SerializeField] float offsetHeight = 0.5f;

    [Header("References")]
    [SerializeField] InteractEvent interaction;
    

    [Header("World Space UI")]
    [SerializeField] GameObject localCanvas;
    [SerializeField] TextMeshProUGUI localTextComponent;
    [SerializeField] Collider2D colliderToFollow;

    void OnValidate()
    {
        FetchReferences();
        SyncUI();
    }

    void Awake()
    {
        localCanvas.SetActive(false);
    }

    public void TogglePrompt(bool show)
    {        
        if (show) SyncUI();
        localCanvas.SetActive(show);
    }

    public void Interact()
    {
        if (interaction != null)
        {
            interaction.Execute();
        }
    }

    public void SetPromptText(string newText)
    {
        promptText = newText;
    }

    void FetchReferences()
    {
        // Find GatedInteraction on this object (or siblings if needed)
        if (interaction == null) interaction = GetComponent<InteractEvent>();

        // Find Canvas in children
        if (localCanvas == null)
        {
            Canvas canvas = GetComponentInChildren<Canvas>(true);
            if (canvas != null) localCanvas = canvas.gameObject;
        }

        // Find Text Component in children
        if (localTextComponent == null)
        {
            localTextComponent = GetComponentInChildren<TextMeshProUGUI>(true);
        }

        // Find Collider for alignment
        if (colliderToFollow == null)
        {
            colliderToFollow = GetComponent<Collider2D>() ?? GetComponentInChildren<Collider2D>();
        }
    }

    void SyncUI()
    {
        if (localTextComponent == null) return;

        // Update Text
        localTextComponent.text = promptText;

        // Update Position based on collider bounds
        if (colliderToFollow != null)
        {
            float topY = colliderToFollow.bounds.max.y;
            localTextComponent.transform.position = new Vector3(
                colliderToFollow.transform.position.x,
                topY + offsetHeight,
                colliderToFollow.transform.position.z
            );
        }
    }
}