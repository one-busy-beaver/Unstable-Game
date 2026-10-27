using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapManager : MonoBehaviour
{
    [SerializeField] private string defaultScene = "MainMenu"; 

    void Start()
    {
        string sceneToLoad = defaultScene;

        #if UNITY_EDITOR
        string lastScene = UnityEditor.EditorPrefs.GetString("LastOpenedScene", "");
        
        // If we have a saved scene and it's not the one we are in right now
        if (!string.IsNullOrEmpty(lastScene) && lastScene != SceneManager.GetActiveScene().name)
        {
            sceneToLoad = lastScene;
        }
        #endif

        Debug.Log($"<color=green>Bootstrap:</color> Loading: <b>{sceneToLoad}</b>");
        SceneManager.LoadScene(sceneToLoad);
    }
}