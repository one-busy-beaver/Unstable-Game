using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{

    [Header("UI Panels")]
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject controlsPanel;
    
    // This is hidden but stays in the metadata for the build
    [HideInInspector] [SerializeField] string initialSceneName;

#if UNITY_EDITOR
    [Header("Scene Configuration")]
    [Tooltip("Drag the first level of your game here.")]
    [SerializeField] SceneAsset initialScene;

    private void OnValidate()
    {
        // Automatically sync the string name whenever you change the asset in the inspector
        if (initialScene != null)
        {
            initialSceneName = initialScene.name;
        }
    }
#endif

    public void PlayGame()
    {
        string sceneToLoad = SceneLoader.Instance.lastRespawnScene;

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            sceneToLoad = initialSceneName;
        }

        SceneLoader.Instance.LoadScene(sceneToLoad, null);
    }

    public void ControlPanel()
    {
        mainMenuPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    public void CloseControlPanel()
    {
        controlsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void ResetBinding()
    {
        // Tell the InputManager to wipe everything
        InputManager.Instance.ResetAllBindings();
        
        // Find all rebind buttons in the panel and tell them to refresh their labels
        RebindButton[] allButtons = controlsPanel.GetComponentsInChildren<RebindButton>();
        foreach (var btn in allButtons)
        {
            btn.RefreshUI();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Exited");
    }
}
