using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class PlayerThrowPredictionPointsObjectPool : ObjectPool
{
    public static PlayerThrowPredictionPointsObjectPool Instance;

    private LayerMask layersToCheck;
    GameObject throwPoint;
    Collider2D[] collisions;

    public override void Awake()
    {
        Instance = this;

        for(int i = 0; i <= 5; i++) { GrowPool(); }
        
        layersToCheck = ~((1 << 0) | (1 << 1) | (1 << 2) | (1 << 4) | (1 << 5) | (1 << 6));
    }

    public void ShowTossTrajectory(Transform tossSpawnPoint, float tossForce)
    {
        ClearToss();
        int counter = 0;
        collisions = null;
        while(collisions == null || counter <=50)
        {
            throwPoint = GetFromPool();
            throwPoint.transform.position = CalcPointPositions(counter * 0.1f, tossSpawnPoint, tossForce);

            collisions = Physics2D.OverlapCircleAll(throwPoint.transform.position, 0.1f, layersToCheck, -.1f, .1f);

            if(collisions != null) 
            { 
                foreach (Collider2D col in collisions) 
                { 
                    Debug.Log("Collided with object: " + col.name); 
                }
            }

            counter++;

            /*if (throwPoint.GetComponent<PlayerThrowPredictionPoints>().HasCollided() == true)
            {   AddToPool(throwPoint); 
                Debug.Log("hasCollided was read in PlayerThrow Object Pool"); 
                break; 
            }*/


        }
    }

    Vector2 CalcPointPositions(float time, Transform tossSpawnPoint, float tossForce)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 transformPos = tossSpawnPoint.position;
        mousePos.z = transformPos.z;
        Vector3 bulletDir = (mousePos - transformPos).normalized;

        Vector2 currentPointPosition = (Vector2)tossSpawnPoint.transform.position + (Vector2)(time * tossForce * bulletDir) + (time * time) * 0.5f * Physics2D.gravity;
        return currentPointPosition;
    }

    public void ClearToss()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            AddToPool(child.gameObject);
        }
    }

}
