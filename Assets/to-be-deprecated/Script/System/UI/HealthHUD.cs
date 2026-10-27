using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthHUD : MonoBehaviour
{
    [SerializeField] private GameObject maskPrefab;
    [SerializeField] private Sprite fullMask;
    [SerializeField] private Sprite emptyMask;

    private List<Image> maskImages = new List<Image>();

    private void OnEnable() => PlayerInventory.OnInventoryChanged += UpdateMasks;
    private void OnDisable() => PlayerInventory.OnInventoryChanged -= UpdateMasks;

    private void Start() => SetupMasks();

    void SetupMasks()
    {
        // Create the initial masks based on maxHealth
        for (int i = 0; i < PlayerInventory.Instance.maxHealth; i++)
        {
            GameObject newMask = Instantiate(maskPrefab, transform);
            maskImages.Add(newMask.GetComponent<Image>());
        }
        UpdateMasks();
    }

    void UpdateMasks()
    {
        for (int i = 0; i < maskImages.Count; i++)
        {
            // If the index is less than health, show full
            maskImages[i].sprite = (i < PlayerInventory.Instance.currentHealth) ? fullMask : emptyMask;
        }
    }
}