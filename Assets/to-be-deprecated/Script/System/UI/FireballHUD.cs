using UnityEngine;
using TMPro;

public class FireballHUD : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI ammoText;

    private void OnEnable()
    {
        // Subscribe to the inventory update event
        PlayerInventory.OnInventoryChanged += RefreshFireballUI;
    }

    private void OnDisable()
    {
        PlayerInventory.OnInventoryChanged -= RefreshFireballUI;
    }

    private void Start()
    {
        RefreshFireballUI();
    }

    private void RefreshFireballUI()
    {
        // Formats the text as "Current / Max" (e.g., "5 / 10")
        int current = PlayerInventory.Instance.currentFireballs;
        int max = PlayerInventory.Instance.maxFireballs;

        ammoText.text = $"{current} / {max}";
    }
}