using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    public GameObject prefabToUse;

    private Queue<GameObject> availablePrefabs = new Queue<GameObject>();


    public virtual void Awake()
    {
        GrowPool();
    }

    public virtual void GrowPool()
    {
        for (int i = 0; i < 10; i++)
        {
            var instanceToAdd = Instantiate(prefabToUse);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }

    public virtual void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        availablePrefabs.Enqueue(instance);
    }

    public virtual GameObject GetFromPool()
    {
        if (availablePrefabs.Count == 0)
        {
            GrowPool();
        }

        var instance = availablePrefabs.Dequeue();
        instance.SetActive(true);
        return instance;
    }
}
