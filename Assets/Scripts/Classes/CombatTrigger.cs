using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides basic functionality for different types of combat triggers
/// </summary>
public class CombatTrigger : MonoBehaviour
{
    // stores the function to call on trigger; set in child classes
    protected Action FunctionCalledOnTrigger;

    // layers the trigger should check for
    [SerializeField] protected LayerMask layersToCheck;

    // used if enemies are using the trigger
    protected EnemyBehaviour enemyBehaviour;

    // needed to get attacking state
    protected Controller controller;

    // to display when the listed enemy is in attack range; for player's this is the enemy, for enemy's this is Player
    [SerializeField] bool _enemyInAttackRange; public bool EnemyInAttackRange { get { return _enemyInAttackRange; } set { _enemyInAttackRange = value; } }

    virtual protected void Start()
    {
        controller = GetComponentInParent<Controller>();

        // if no value is set in layer mask, assume this is an enemy and get the enemy behaviour and set the collision mask to Player
        if (layersToCheck.value == 0 &&
            GetComponentInParent<EnemyBehaviour>() != null)
        {
            enemyBehaviour = GetComponentInParent<EnemyBehaviour>();
            layersToCheck = LayerMask.GetMask("Player");
        }
    }

    // if not attacking and enemy in range, then attack
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (EnemyInAttackRange && !controller.IsAttacking) { FunctionCalledOnTrigger(); }
        else { CheckColliderLayer(collider); }
    }

    // tracks when an 'enemy' enters and marks true if so
    private void OnTriggerEnter2D(Collider2D collider) { if (CheckColliderLayer(collider)) { EnemyInAttackRange = true; Debug.Log("Enemy entering projectile range"); } }

    // tracks when an 'enemy' exits and marks false if so
    private void OnTriggerExit2D(Collider2D collider) { if (CheckColliderLayer(collider)) { EnemyInAttackRange = false; } Debug.Log("Enemy leaving projectile range"); }

    // returns true if collider is in a layer to check for
    bool CheckColliderLayer(Collider2D collider)
    {
        return (layersToCheck.value & (1 << collider.gameObject.layer)) != 0;
    }
}
