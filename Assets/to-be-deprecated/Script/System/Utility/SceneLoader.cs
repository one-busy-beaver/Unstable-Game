using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    public string lastRespawnScene; // variable to remember where to send the player

    [SerializeField] GameObject HUDPanel;
    [SerializeField] GameObject playerPrefab;
    
    ExitSpawnID targetSpawnID;

    private void Awake()
    {
        // Only allow one SceneLoader to exist
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded; // Connect the method to the Unity Engine
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded; // Disconnect it when the object is destroyed

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "_Bootstrap") return;

        // Find every SpawnPoint component in the new scene
        SpawnPoint[] allSpawns = FindObjectsByType<SpawnPoint>(FindObjectsInactive.Exclude);
        SpawnPoint targetSpawn = null;
        SpawnPoint taggedSpawn = null;

        // Look for the specific ID and "Respawn" Tag
        foreach (SpawnPoint sp in allSpawns)
        {
            if (sp.spawnID == targetSpawnID)
            {
                targetSpawn = sp;
            }

            if (sp.CompareTag("Respawn"))
            {
                taggedSpawn = sp;
                lastRespawnScene = scene.name;
            }
        }

        // specific ID > "Respawn" Tag
        SpawnPoint selectedSpawn = targetSpawn? targetSpawn : taggedSpawn;

        // FALLBACK: If ID not found or empty, take the first one available
        if (selectedSpawn == null && allSpawns.Length > 0)
        {
            selectedSpawn = allSpawns[0];
        }

        // Spawn the Player
        if (selectedSpawn != null)
        {
            GameObject newPlayer = Instantiate(playerPrefab, selectedSpawn.transform.position, selectedSpawn.transform.rotation);
            
            CameraFollow follow = FindAnyObjectByType<CameraFollow>();
            if (follow != null)
            {
                follow.SetTarget(newPlayer);
            }
        }
        else
        {
            Debug.Log($"SceneLoader: No SpawnPoints found in {scene.name}!");
        }

        targetSpawnID = null; // Reset for next time
    }

    public void LoadScene(string sceneToLoad, ExitSpawnID pointToSpawn)
    {
        targetSpawnID = pointToSpawn;
        SceneManager.LoadSceneAsync(sceneToLoad.ToString()); // Async does not pause the current scene
        if (sceneToLoad != "Main_Menu") HUDPanel.SetActive(true);
    }

    public void LoadMainMenu()
    {
        // Reset session pickups and enemies
        WorldState.Instance.ResetSession();

        // Restore player health and fireballs
        PlayerInventory.Instance.UpdateHealth(5);
        PlayerInventory.Instance.UpdateFireballs(-10);

        HUDPanel.SetActive(false);

        SceneManager.LoadSceneAsync("Main_Menu");
    }

}
