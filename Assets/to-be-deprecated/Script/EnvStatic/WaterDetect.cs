using UnityEngine;

public class WaterDetect : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerSwim swimScript))
        {
            swimScript.EnterWater();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerSwim swimScript))
        {
            swimScript.ExitWater();
        }
    }
}