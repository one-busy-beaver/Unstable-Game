using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewID", menuName = "Data/Exit Spawn ID")]
public class ExitSpawnID: ScriptableObject
{
    // This is the string your game will actually use to load the scene at runtime.
    [HideInInspector] public string sceneNameA;
    [HideInInspector] public string sceneNameB;

#if UNITY_EDITOR
    [Header("Connected Scenes")]
    [SerializeField] SceneAsset sceneA;
    [SerializeField] SceneAsset sceneB;

    private void OnValidate()
    {
        sceneNameA = sceneA != null ? sceneA.name : string.Empty;
        sceneNameB = sceneB != null ? sceneB.name : string.Empty;
    }
#endif

    public string GetDestinationScene(string currentScene)
    {
        if (currentScene == sceneNameA) return sceneNameB;
        if (currentScene == sceneNameB) return sceneNameA;
        
        return string.Empty; 
    }
}