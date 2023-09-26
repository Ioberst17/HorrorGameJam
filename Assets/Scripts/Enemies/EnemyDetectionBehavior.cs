using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component on enemies used to detect whether player is detected
/// </summary>
public class EnemyDetectionBehavior : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    public bool manualController;

    void Start()
    {
        if (!manualController) { enemyController = GetComponentInParent<EnemyController>(); }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        // if playerInZone isn't true and player has collided, mark playerInZone as true
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player")) { if (!enemyController.PlayerInZone) { enemyController.PlayerInZone = true; } }

    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        // mark when player has left
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player")) { enemyController.PlayerInZone = false; }
    }
}
