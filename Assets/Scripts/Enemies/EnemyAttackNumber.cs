using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackNumber : MonoBehaviour
{
    public int enemyAttackNumber;

    private void OnTriggerStay2D(Collider2D collision) { if (collision.gameObject.GetComponent<PlayerShield>() != null) { } }
}
