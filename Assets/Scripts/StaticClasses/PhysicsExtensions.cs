using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsExtensions
{
    public static void Flip(GameObject gameObject)
    {
        gameObject.transform.localScale = new Vector3(
            gameObject.transform.localScale.x,
            gameObject.transform.localScale.y * -1,
            gameObject.transform.localScale.z);
    }
}
