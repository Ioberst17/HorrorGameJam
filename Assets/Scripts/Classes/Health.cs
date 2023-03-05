using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Health : MonoBehaviour, IDamageable
{
    public int HP;
    public int maxHealth;
    public void AddHealth(int healthToAdd)
    {
        if ((HP += healthToAdd) < maxHealth) { HP += healthToAdd; }
        else { HP = maxHealth; }
    }
    public void TakeDamage() { }

    public void HPZero() { }
}