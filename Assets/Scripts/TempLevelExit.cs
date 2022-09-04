using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempLevelExit : MonoBehaviour
{
    public SceneManagerIndexBased_Mason SceneManagerIndexBased;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer
                == LayerMask.NameToLayer("Player"))
        {

            SceneManagerIndexBased.ChangeScene();
        }

    }
}
