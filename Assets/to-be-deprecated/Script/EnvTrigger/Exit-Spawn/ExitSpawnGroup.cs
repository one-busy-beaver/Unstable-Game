using UnityEngine;

public class ExitSpawnGroup : MonoBehaviour
{
    [Header("Assign ID Here to Auto-Fill Children")]
    public ExitSpawnID exitSpawnID;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (exitSpawnID == null) return;

        SceneExit exit = GetComponentInChildren<SceneExit>();
        if (exit != null)
        {
            exit.targetSpawnID = exitSpawnID;
        }

        SpawnPoint spawn = GetComponentInChildren<SpawnPoint>();
        if (spawn != null)
        {
            spawn.spawnID = exitSpawnID;
        }
    }
#endif
}