using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class PlaytestTracker
{
    private const string BootstrapScenePath = "Assets/Scene/_Bootstrap.unity"; // Ensure this path is correct
    private const string LastSceneKey = "LastOpenedScene";

    static PlaytestTracker()
    {
        //EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            // 1. Save the current scene name so the BootstrapManager knows where to go
            string currentScene = EditorSceneManager.GetActiveScene().name;
            if (currentScene != "_Bootstrap" && currentScene != "Main_Menu")
            {
                EditorPrefs.SetString(LastSceneKey, currentScene);

                // 2. Force the Bootstrap scene to be the one that actually starts
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(BootstrapScenePath);
                }
            }
        }

        // Optional: Return to the previous scene when stopping play mode
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            string lastScene = EditorPrefs.GetString(LastSceneKey, "");
            if (!string.IsNullOrEmpty(lastScene))
            {
                // Logic to reload the scene you were working on goes here if desired
            }
        }
    }
}