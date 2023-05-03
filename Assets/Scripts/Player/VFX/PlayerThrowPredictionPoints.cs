using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrowPredictionPoints : MonoBehaviour
{

    private bool hasCollided;
    public bool HasCollided() { return hasCollided; }

    void OnEnable()
    {
        hasCollided = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // ignore non-interaction + player physics layers, 3rd layer should map to environment which it sould interact with
        if (other.gameObject.layer >= 6 || other.gameObject.layer == 3)  {  hasCollided = true;  }
    }

    private void OnDisable()
    {
        hasCollided = false;
    }

}
