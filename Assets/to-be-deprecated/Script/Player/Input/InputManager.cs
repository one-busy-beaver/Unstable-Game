using UnityEngine;
using UnityEngine.InputSystem;


public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public PlayerControls Controls { get; private set; }

    private const string RebindsKey = "InputRebinds";

    private void Awake()
    {
        // Only allow one InputManager to exist
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Controls = new PlayerControls();
        LoadRebinds(); // Load saved keys on startup
    }

    private void OnEnable()
    {
        // Use conditional access to prevent errors if Enable is called during setup
        Controls?.Enable();
    }

    private void OnDisable()
    {
        // The null-conditional operator (?.) prevents the NullReferenceException
        Controls?.Disable();
    }

    public void StartRebind(string actionName, int bindingIndex, System.Action onUIUpdate)
    {
        // Find the action (e.g., "Jump" or "Move")
        InputAction action = Controls.asset.FindAction(actionName);
        
        if (action == null || action.bindings.Count <= bindingIndex) return;

        // Disable controls during rebind to prevent ghost inputs
        action.Disable();

        var rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("<Mouse>/leftButton") // Ignore the click used to start rebinding
            .OnMatchWaitForAnother(0.1f) // Short delay to prevent accidental double-inputs
            .OnComplete(operation =>
        {
            CleanUpRebind(operation, action, onUIUpdate);
            SaveRebinds();
        })
        .OnCancel(operation =>
        {
            CleanUpRebind(operation, action, onUIUpdate);
        });

        rebindOperation.Start();
    }

    private void CleanUpRebind(InputActionRebindingExtensions.RebindingOperation operation, InputAction action, System.Action onUIUpdate)
{
    operation.Dispose();
    action.Enable();
    onUIUpdate?.Invoke();
}

    private void SaveRebinds()
    {
        string rebinds = Controls.asset.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(RebindsKey, rebinds);
        PlayerPrefs.Save();
    }

    private void LoadRebinds()
    {
        string rebinds = PlayerPrefs.GetString(RebindsKey);
        if (!string.IsNullOrEmpty(rebinds))
        {
            Controls.asset.LoadBindingOverridesFromJson(rebinds);
        }
    }

    public string GetBindingDisplay(string actionName, int bindingIndex)
    {
        InputAction action = Controls.asset.FindAction(actionName);
        return action?.GetBindingDisplayString(bindingIndex) ?? "";
    }

    public void ResetAllBindings()
    {
        // 1. Clear all active overrides in the Input System
        Controls.asset.RemoveAllBindingOverrides();

        // 2. Wipe the saved data from disk
        PlayerPrefs.DeleteKey("InputRebinds"); 
        PlayerPrefs.Save();
    }
}
