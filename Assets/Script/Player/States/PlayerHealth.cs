using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float deathDelay = 1.5f; // Time for animation to play

    Animator anim;
    Rigidbody2D rb;

    bool isDead = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleDeath();
    }

    void HandleDeath()
    {
        if (PlayerInventory.Instance.currentHealth <= 0 && !isDead) 
        {
            isDead = true;
            anim.SetTrigger("Die"); // Transition from Any State
            
            // Lock physics and input
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false; 

            // Stop other scripts
            if (TryGetComponent<PlayerWalk>(out var walk)) walk.enabled = false;
            if (TryGetComponent<PlayerJump>(out var jump)) jump.enabled = false;

            StartCoroutine(LoadMenuAfterDelay());
        }
    }

    IEnumerator LoadMenuAfterDelay()
    {
        yield return new WaitForSeconds(deathDelay); // Wait for animation
        SceneLoader.Instance.LoadMainMenu();
    }

    public void TakeDamage(int damage)
    {
        PlayerInventory.Instance.UpdateHealth(-damage);
        
        if (PlayerInventory.Instance.currentHealth > 0)
        {
            anim.SetTrigger("Hurt");
        }
    }
}