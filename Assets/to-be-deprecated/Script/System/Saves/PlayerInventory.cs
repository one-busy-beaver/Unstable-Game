using UnityEngine;
using System;
using Unity.VisualScripting;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;
    public static event Action OnInventoryChanged;

    [Header("Player Stats")]
    public int currentHealth = 5;
    public int maxHealth = 5;

    [Header("Ammo")]
    public int currentFireballs = 0; // Player starts at 0
    public int maxFireballs = 10;

    [Header("Combat Stats")]
    public int baseDamage = 2;
    public int courageDamage = 5;
    public int currentDamage = 2; // The one enemies actually read

    [Header("Unlocked Abilities")]
    [SerializeField] bool _hasDash = false;
    [SerializeField] bool _hasDoubleJump = false;
    [SerializeField] bool _hasSwim = false;

    [Header("Unlocked Items")]
    [SerializeField] bool _hasBaseKey = false;
    [SerializeField] bool _hasCourage = false;

    // Public "Getters"
    public bool HasDash => _hasDash;
    public bool HasDoubleJump => _hasDoubleJump;
    public bool HasSwim => _hasSwim;
    public bool HasBaseKey => _hasBaseKey;
    public bool HasCourage => _hasCourage;

    void Awake() 
    { 
        if (Instance == null) 
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Call this from your ItemPickup script
    public void Unlock(CollectID ability)
    {
        switch (ability)
        {
            case CollectID.Dash: _hasDash = true; break;
            case CollectID.DoubleJump: _hasDoubleJump = true; break;
            case CollectID.Swim: _hasSwim = true; break;
            case CollectID.BaseKey: _hasBaseKey = true; break;
            case CollectID.Courage: 
                _hasCourage = true; 
                currentDamage = courageDamage; // Boost damage immediately
                break;
        }

        // Ring the bell so the sword turns yellow instantly!
        OnInventoryChanged?.Invoke();
    }

    public void UpdateHealth(int amount)
    {
        // Clamp health between 0 and maxHealth
        // currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth)
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        if (currentHealth < 0) currentHealth = 0;

        OnInventoryChanged?.Invoke();
    }

    public void UpdateFireballs(int amount)
    {
        currentFireballs += amount;
        if (currentFireballs > maxFireballs) currentFireballs = maxFireballs;
        if (currentFireballs < 0) currentFireballs = 0;

        OnInventoryChanged?.Invoke();
    }
}