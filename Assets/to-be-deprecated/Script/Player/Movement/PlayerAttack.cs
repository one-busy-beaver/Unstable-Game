using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attacking Settings")]
    
    [SerializeField] GameObject attackObject;
    [SerializeField] LayerMask attackableLayer;
    [SerializeField] float timeBetweenAttack = 0.5f; // prevent infinite damage

    [SerializeField] private AudioClip hitSound;
    AudioSource audioSource;


    bool attack = false;    
    float timeSinceAttack;

    // Player components
    Animator anim;
    PlayerStates pState;
    Recoil recoil;

    void Start()
    {
        anim = GetComponent<Animator>();
        pState = GetComponent<PlayerStates>();
        recoil = GetComponent<Recoil>();
        attackObject.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        attack = InputManager.Instance.Controls.Player.Attack.IsPressed();
        Attack();
    }

    void Attack()
    {
        timeSinceAttack += Time.deltaTime;

        if (attack && timeSinceAttack >= timeBetweenAttack && !pState.isClimbing) {
            timeSinceAttack = 0;
            anim.SetTrigger("Attacking");

            ProcessHit();
        }
    }

    void ProcessHit()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(attackableLayer);
        filter.useLayerMask = true;
        filter.useTriggers = true;

        List<Collider2D> results = new List<Collider2D>();
        attackObject.SetActive(true);

        PolygonCollider2D attackHitbox = attackObject.GetComponent<PolygonCollider2D>();
        int hitCount = attackHitbox.Overlap(filter, results);

        if (hitCount > 0) 
        {
            Debug.Log("I'm hitting something");
            audioSource.PlayOneShot(hitSound);
        }

        int damage = PlayerInventory.Instance.currentDamage;
        foreach (Collider2D curObj in results)
        {
            Vector2 attackDir = new Vector2(transform.localScale.x, 0);

            Enemy enemy = curObj.GetComponent<Enemy>();
            if (enemy != null) 
            {
                recoil.TriggerRecoil((Vector2)transform.position + attackDir);
                enemy.EnemyHit(damage, transform.position);
            }

            Breakable breakable = curObj.GetComponent<Breakable>();
            if (breakable != null)
            {
                breakable.ObjectHit(damage);
            }
        }
        attackObject.SetActive(false);
    }
}