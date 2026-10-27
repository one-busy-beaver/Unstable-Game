using UnityEngine;

public class PlayerCast : MonoBehaviour
{
    [Header("Casting Settings")]
    [SerializeField] GameObject fireballPrefab;
    [SerializeField] Transform launchPoint; // Create an empty child object where the fireball starts
    [SerializeField] float castCooldown = 0.8f;
 
    Animator anim;
    float cooldownTimer;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (InputManager.Instance.Controls.Player.Cast.triggered && 
            cooldownTimer >= castCooldown &&
            PlayerInventory.Instance.currentFireballs > 0)
        {
            Cast();
        }
    }

    void Cast()
    {
        cooldownTimer = 0;
        PlayerInventory.Instance.UpdateFireballs(-1);

        anim.SetTrigger("Casting");

        GameObject ball = Instantiate(fireballPrefab, launchPoint.position, Quaternion.identity);
        float direction = transform.localScale.x;
        ball.GetComponent<Fireball>().Setup(direction);
        
    }
}