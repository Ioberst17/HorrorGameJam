using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellhoundAttackTrigger : MonoBehaviour
{
    private HellhoundBehavior HellhoundBehavior;

    // Start is called before the first frame update
    void Start()
    {
        HellhoundBehavior = GetComponentInParent<HellhoundBehavior>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer
                == LayerMask.NameToLayer("Player"))
        {
            HellhoundBehavior.PounceTrigger();
        }
    }
}