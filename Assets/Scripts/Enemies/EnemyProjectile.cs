using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    private EnemyDatabase enemyDatabase;
    public int attackNumber;
    public int damageValue;
    
    // Start is called before the first frame update
    virtual public void Start()
    {
        enemyDatabase = GameObject.Find("EnemyDatabase").GetComponent<EnemyDatabase>();
        GetDamage();
    }

    public virtual void GetDamage() { damageValue = enemyDatabase.GetAttackDamage(gameObject.tag, attackNumber); }
}
