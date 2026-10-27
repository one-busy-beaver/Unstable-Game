using UnityEngine;

public class UniquePlayer : MonoBehaviour
{
    public static UniquePlayer Instance;
    public bool isMainPlayer;
    
    void Awake()
    {
        if (!isMainPlayer)
        {
            Destroy(gameObject);
            return;
        }

        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
}
