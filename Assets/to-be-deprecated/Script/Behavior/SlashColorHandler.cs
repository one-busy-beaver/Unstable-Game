using UnityEngine;

public class SlashColorHandler : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color courageColor = Color.yellow;

    private void OnEnable()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        // Swap color instantly if player has courage
        if (PlayerInventory.Instance != null && PlayerInventory.Instance.HasCourage)
        {
            spriteRenderer.color = courageColor;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }
}