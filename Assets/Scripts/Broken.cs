using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Broken : MonoBehaviour
{
    private Transform[] children;
    private float torque, DirX, DirY;
    private void Start()
    {
        int BrokenEnviroLayer = LayerMask.NameToLayer("BrokenEnviro");
        gameObject.layer = BrokenEnviroLayer;
        children = GetComponentsInChildren<Transform>();

        foreach (Transform child in children)
        {
            child.gameObject.layer = BrokenEnviroLayer;
            if (child.GetComponent<Rigidbody2D>() == null) { child.AddComponent<Rigidbody2D>(); }
            if (child.GetComponent<Collider2D>() == null) 
            { child.AddComponent<BoxCollider2D>(); Debug.Log("Make sure proper colliders are on the children of the attached game object with name of: " + name); }

            AddRandomForce(child.GetComponent<Rigidbody2D>());
        }
        Destroy(gameObject, 2f);
    }

    private void AddRandomForce(Rigidbody2D rb)
    {
        float torqueForce = 100f;
        float dirForce = 100f;

        torque = Random.Range(-torqueForce, torqueForce);
        DirX = Random.Range(-dirForce, dirForce);
        DirY = Random.Range(-dirForce, dirForce);
        rb.AddTorque(torque);
        rb.AddForce(new Vector2(DirX, DirY));
    }
}
