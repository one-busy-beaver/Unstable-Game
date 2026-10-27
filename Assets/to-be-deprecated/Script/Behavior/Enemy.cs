using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    [SerializeField] List<GameObject> sprites;
    [SerializeField] SessionID enemyID;
    [SerializeField] int health = 6;
    [SerializeField] int contactDamage = 1;
    [SerializeField] float contactForce = 6f;

    [SerializeField] Color deathColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    [SerializeField] List<Collider2D> damageHitboxes;

    Recoil recoil;
    protected bool isDead = false;

    void Awake()
    {
        if (WorldState.Instance.IsDead(enemyID))
        {
            // If dead, just disable it immediately or set to death state
            gameObject.SetActive(false); 
            return;
        }

        recoil = GetComponent<Recoil>();

        // Ensure all assigned colliders are triggers
        foreach (var col in damageHitboxes)
        {
            col.isTrigger = true;
        }
    }

    protected void Update() => HandleDeath();

    public void EnemyHit(int damage, Vector2 sourcePos)
    {
        health -= damage;
        recoil.TriggerRecoil(sourcePos);
    }

    void HandleDeath()
    {
        if (isDead) return;
        
        if (health <= 0) 
        {
            isDead = true;
            WorldState.Instance.MarkAsDead(enemyID);

            // Turn the sprite gray
            foreach (var sprite in sprites)
            {
                sprite.GetComponent<SpriteRenderer>().color = deathColor;
            }
            
            // Disable all hitboxes so it can't hurt the player or be hit again
            foreach (var col in damageHitboxes)
            {
                col.enabled = false;
            }

            // Optional: If you have a separate movement/AI script, 
            // you should disable it here as well.
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Look for a PlayerHealth component on contact
        if (collision.TryGetComponent(out PlayerHealth player))
        {
            player.TakeDamage(contactDamage);
            
            if (collision.TryGetComponent(out Recoil playerRecoil))
            {
                playerRecoil.TriggerRecoil(transform.position, false, contactForce);
            }
        }
    }
}