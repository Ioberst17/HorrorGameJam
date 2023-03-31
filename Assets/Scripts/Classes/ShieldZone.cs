using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShieldZone
{
    [Range(-180, 180f)] public float minAngle, maxAngle;
    [Range(0f, 1f)] public float damageAbsorption, knockbackAbsorption;

    public ShieldZone()
    {

    }
}
