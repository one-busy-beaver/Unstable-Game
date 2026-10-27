using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class RebindButton : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string actionName;      // e.g., "Jump" or "Move"
    [SerializeField] private int bindingIndex;       // 0 for buttons, 1-4 for WASD
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI actionLabel; // "Jump"
    [SerializeField] private Button selfButton;

    private TMP_Text keyLabel;    // "Space"

    private void OnValidate()
    {
        keyLabel = selfButton.GetComponentInChildren<TMPro.TMP_Text>();
        RefreshUI();
    }

    private void Start()
    {
        keyLabel = selfButton.GetComponentInChildren<TMPro.TMP_Text>();
        RefreshUI();
        selfButton.onClick.AddListener(DoRebind);
    }

    public void RefreshUI()
    {
        actionLabel.text = actionName;
        if (Application.isPlaying)
        {
            var (effectiveName, effectiveIndex) = ResolveBinding();
            keyLabel.text = InputManager.Instance.GetBindingDisplay(effectiveName, effectiveIndex);
        }
        else
        {
            GetKeyLabel();
        }
    }

    private void DoRebind()
    {
        selfButton.interactable = false;
        keyLabel.text = "Press any key...";

        var (effectiveName, effectiveIndex) = ResolveBinding();
        InputManager.Instance.StartRebind(effectiveName, effectiveIndex, RefreshUI);

        selfButton.interactable = true;
    }

    private (string name, int index) ResolveBinding()
    {
        string lowerName = actionName.ToLower();
        switch (lowerName)
        {
            case "up":    return ("Move", 1);
            case "down":  return ("Move", 2);
            case "left":  return ("Move", 3);
            case "right": return ("Move", 4);
            default:      return (actionName, bindingIndex);
        }
    }

    private void GetKeyLabel()
    {
        PlayerControls tempControls = new PlayerControls();

        // Load saved overrides so the Editor shows your custom keys, not just defaults
        string savedRebinds = PlayerPrefs.GetString("InputRebinds", string.Empty);
        if (!string.IsNullOrEmpty(savedRebinds))
        {
            tempControls.asset.LoadBindingOverridesFromJson(savedRebinds);
        }

        var (effectiveName, effectiveIndex) = ResolveBinding();
        var action = tempControls.asset.FindAction(effectiveName);
        if (action != null && effectiveIndex < action.bindings.Count)
        {
            keyLabel.text = action.GetBindingDisplayString(effectiveIndex);
        }
        else
        {
            keyLabel.text = "Key";
        }

        if (Application.isPlaying)
        {
            tempControls.Dispose();
        }
        else
        {
            DestroyImmediate(tempControls.asset);
        }
    }
}