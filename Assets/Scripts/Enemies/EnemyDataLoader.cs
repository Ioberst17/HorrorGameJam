using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Loads in enemy data from relevant databases, so they can be used by enemy's components
/// </summary>
public class EnemyDataLoader : MonoBehaviour
{
    // An event was added due to timing issues; this is used for EnemyAttackManager to be able to load data in from attacks
    // And not have it be empty on load
    public event Action DataLoaded;
    // used to get data
    EnemyDatabase enemyDatabase;
    EnemyAttackDatabase enemyAttackDatabase;
    EnemyProjectileDatabase enemyProjectileDatabase;

    // used to store enemyID from enemyDatabase
    int enemyID { get; set; } = -1;

    // publically accessibly information
    public List<Projectile> projectiles = new List<Projectile>();
    public List<Attack> attacks = new List<Attack>();
    public EnemyData data;

    private void Start()
    {
        enemyDatabase = FindObjectOfType<EnemyDatabase>();
        enemyAttackDatabase = FindObjectOfType<EnemyAttackDatabase>();
        enemyProjectileDatabase = FindObjectOfType<EnemyProjectileDatabase>();
        GetBaseEnemyData();
        GetEnemyAttackAndProjectileData();
    }

    void GetBaseEnemyData()
    {
        if (GetComponentInChildren<HellHoundBehaviour>()) { enemyID = 0; } // add enemyID as in enemy database + behavior component
        else if (GetComponentInChildren<BatBehaviour>()) { enemyID = 1; }
        else if (GetComponentInChildren<ParalysisDemonBehaviour>()) { enemyID = 2; }
        else if (GetComponentInChildren<SpiderBehaviour>()) { enemyID = 3; }
        else if (GetComponentInChildren<BloodGolemBehaviour>()) { enemyID = 4; }
        else if (GetComponentInChildren<GargoyleBehaviour>()) { enemyID = 5; }
        else if (GetComponentInChildren<DeathBringerBehaviour>()) { enemyID = 7; }
        else if (GetComponentInChildren<FireWyrmBehaviour>()) { enemyID = 8; }
        else if (GetComponentInChildren<IceWyrmBehaviour>()) { enemyID = 9; }
        else if (GetComponentInChildren<ArcaneArcherBehaviour>()) { enemyID = 10; }
        else if (GetComponentInChildren<DarkMageBehaviour>()) { enemyID = 11; }
        else if (GetComponentInChildren<FlameMageBehaviour>()) { enemyID = 12; }
        else { enemyID = -1; }

        if (enemyID != -1) { data = enemyDatabase.data.entries[enemyID]; }
        else { Debug.Log("Enemy ID is being loaded in as -1 - check to make sure behavior is found"); }
    }

    void GetEnemyAttackAndProjectileData()
    {
        foreach(var attack in enemyAttackDatabase.data.entries)
        {
            if(attack.owner == data.name)  { attacks.Add(attack); }
        }        
        foreach(var projectile in enemyProjectileDatabase.data.entries)
        {
            if(projectile.owner == data.name)  { projectiles.Add(projectile); }
        }
        DataLoaded?.Invoke();
    }
}
