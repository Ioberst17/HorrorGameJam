using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTrigger : MonoBehaviour
{
    [SerializeField] protected LayerMask layersToCheck;
    protected EnemyBehaviour enemyBehaviour;
    protected Controller controller;
    public bool enemyInAttackRange;

    void Start()
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
        if (enemyInAttackRange = true && !controller.IsAttacking) { enemyBehaviour.ProjectileTrigger(); }
    }

    // tracks when an 'enemy' enters and marks true if so
    private void OnTriggerEnter2D(Collider2D collider) { { if (CollisionToWatchForHasHappened(collider)) { enemyInAttackRange = true; } } }

    // tracks when an 'enemy' exits and marks false if so
    private void OnTriggerExit2D(Collider2D collider) { if (CollisionToWatchForHasHappened(collider)) { enemyInAttackRange = false; } }

    // returns true if collider is in a layer to check for
    bool CollisionToWatchForHasHappened(Collider2D collider)
    {
        return (layersToCheck.value & (1 << collider.gameObject.layer)) != 0;
    }
}
