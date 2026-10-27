using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] float speed = 15f;
    [SerializeField] int damage = 10;
    [SerializeField] int courageDamage = 20;
    [SerializeField] float sizeMultiplier = 1.5f;
    [SerializeField] float lifetime = 1f;

    [SerializeField] private AudioClip hitSound;
    AudioSource audioSource;

    Rigidbody2D rb;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Destroy(gameObject, lifetime); // Self-destruct after a few seconds
    }

    public void Setup(float direction)
    {
        // Check if the player has collected Courage
        bool isBoosted = PlayerInventory.Instance != null && PlayerInventory.Instance.HasCourage;

        // 1. Increase Damage
        if (isBoosted) damage = courageDamage;

        // 2. Increase Size (Multiplying direction ensures it still flips correctly)
        float finalSize = isBoosted ? sizeMultiplier : 1f;
        transform.localScale = new Vector3(direction * finalSize, finalSize, 1);

        // 3. Match the yellow SlashEffect color
        if (isBoosted) GetComponent<SpriteRenderer>().color = Color.yellow;

        // Physics setup
        rb.linearVelocity = new Vector2(direction * speed, 0);
        rb.gravityScale = 0f;
        }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Check for Enemy
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
            enemy.EnemyHit(damage, transform.position);
            Destroy(gameObject);
            return; // Exit early so we don't check ground
        }

        // 2. Check for Breakable (using the same logic as your attack)
        // If your breakables use a different script name, swap "Breakable" here.
        Breakable breakable = collision.GetComponent<Breakable>();
        if (breakable != null)
        {
            // Call the same method your sword uses to break objects
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
            breakable.ObjectHit(damage); 
            Destroy(gameObject);
            return;
        }
    
    // 3. Destroy if it hits ground
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}