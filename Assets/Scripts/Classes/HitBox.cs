using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HitBox 
{
    [SerializeField] public Vector3 point1; // upper left corner of attack
    [SerializeField] public Vector3 point2; // bottom right corner of attack

    public HitBox(Vector3 point1, Vector3 point2)
    {
        this.point1 = point1;
        this.point2 = point2;
    }
}
