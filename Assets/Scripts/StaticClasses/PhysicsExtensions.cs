using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsExtensions
{
    public static void FlipYScaleAxis(GameObject gameObject)
    {
        gameObject.transform.localScale = new Vector3(
            gameObject.transform.localScale.x,
            gameObject.transform.localScale.y * -1,
            gameObject.transform.localScale.z);
    }    
    
    public static void FlipScale(GameObject gameObject, bool flipScaleOnXAxis = false,bool flipScaleOnYAxis = false,bool flipScaleOnZAxis = false)
    {
        float xChange = 1;
        float yChange = 1;
        float zChange = 1;
        if (flipScaleOnXAxis) { xChange = -1; }
        if (flipScaleOnYAxis) { yChange = -1; }
        if (flipScaleOnZAxis) { zChange = -1; }

        gameObject.transform.localScale = new Vector3(
            gameObject.transform.localScale.x * xChange,
            gameObject.transform.localScale.y * yChange,
            gameObject.transform.localScale.z * zChange);
    }
}
