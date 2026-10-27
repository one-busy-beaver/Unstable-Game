using UnityEngine;

public class DontDestroyMaster : MonoBehaviour
{
    private static DontDestroyMaster instance;

    void Awake() 
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
