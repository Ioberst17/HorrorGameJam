using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller))]
public abstract class Health : MonoBehaviour, IDamageable
{
    // External References
    protected Shield shield; // used for receiving damage input on player

    [Header("Variables To Manage")]
    public int damageTaken;
    [SerializeField] protected int _maxHealth; public int MaxHealth { get { return _maxHealth; } set { _maxHealth = value; } }
    [SerializeField] protected int _hp; public int HP { get { return _hp; } set { _hp = value; } }
    protected float StandardInvincibilityLength { get; set; } = 1.5f;

    void Awake()
    {
        HP = MaxHealth;
    }

    virtual public void AddHealth(int healthToAdd)
    {
        if ((HP += healthToAdd) < MaxHealth) { HP += healthToAdd; }
        else { HP = MaxHealth; }
    }
    virtual public void Hit() { }

    virtual public void HPZero() { }
}
