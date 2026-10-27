using UnityEngine;

public class SceneExit : InteractEvent
{
    public ExitSpawnID targetSpawnID;

    void Reset()
    {
        autoTrigger = true;
    }

    private void Awake() => GetComponent<BoxCollider2D>().isTrigger = true;
    

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (autoTrigger && other.CompareTag("Player"))
        {
            Execute();
        }
    }

    public override void Execute()
    {
        if (targetSpawnID == null)
        {
            Debug.LogError($"Exit setup is incomplete on {gameObject.name}!");
            return;
        }

        // Get the active scene of this exit to determine where to go next
        string currentScene = gameObject.scene.name;
        string destinationScene = targetSpawnID.GetDestinationScene(currentScene);

        if (string.IsNullOrEmpty(destinationScene))
        {
            Debug.LogError($"Current scene '{currentScene}' is not part of the targetSpawnID {targetSpawnID.name}!");
            return;
        }

        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadScene(destinationScene, targetSpawnID);
        else
            Debug.LogError("SceneLoader missing!");
    }
}