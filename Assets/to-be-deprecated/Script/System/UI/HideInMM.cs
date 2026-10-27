using UnityEngine;
using UnityEngine.SceneManagement;

public class HideInMM : MonoBehaviour
{
    

    // Add this to HealthHUD and FireballHUD
    private void Update()
    {
        // Hide the HUD if we are in the Main Menu
        if (SceneManager.GetActiveScene().name == "Main_Menu")
        {
            gameObject.SetActive(false);
        } 
        else
        {
            gameObject.SetActive(true);
        }
    }
}
