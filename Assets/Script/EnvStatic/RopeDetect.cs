using UnityEngine;

public class RopeDetect : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerClimb climbScript = collision.GetComponent<PlayerClimb>();
            if (climbScript != null)
            {
                climbScript.EnterRope(transform);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerClimb climbScript = collision.GetComponent<PlayerClimb>();
            if (climbScript != null)
            {
                climbScript.LeaveRope();
            }
        }
    }
}