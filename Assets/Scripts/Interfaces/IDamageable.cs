using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{

    void Hit() { }

    void Hit(GameObject isHitBy) { }
    void Hit(int damage, Vector3 attackingObjectPosition) { }

    void Hit(int damage, Vector3 attackingObjectPosition, GameObject isHitBy) { }

    void HPZero() { }
}
