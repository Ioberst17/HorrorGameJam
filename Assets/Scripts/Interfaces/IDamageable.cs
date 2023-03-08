using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void Hit(int damage, Vector3 attackingObjectPosition) { }

    void HPZero() { }
}
