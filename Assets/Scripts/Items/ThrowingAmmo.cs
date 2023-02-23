using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Ammo))]
[RequireComponent(typeof(Rigidbody2D))]
public class ThrowingAmmo : MonoBehaviour
{
    float minTorque = .2f;
    float maxTorque = 1f;

    void Awake(){ GetComponent<Rigidbody2D>().AddTorque(Random.Range(minTorque, maxTorque), ForceMode2D.Impulse); }

    

}
