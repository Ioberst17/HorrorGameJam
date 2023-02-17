using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImageObjectPool : MonoBehaviour
{
    [SerializeField]
    private GameObject afterImagePrefab;

    private Queue<GameObject> availableAfterImages = new Queue<GameObject>();

    public static PlayerAfterImageObjectPool Instance { get; private set; }

    public float distanceBetweenAfterImages;
    private float lastAfterImageXPosition;

    private void Awake()
    {
        Instance = this;
        GrowPool();
    }

    private void GrowPool()
    {
        for(int i = 0; i<10; i++)
        {
            var instanceToAdd = Instantiate(afterImagePrefab);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }

    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableAfterImages.Enqueue(instance);
    }

    public GameObject GetFromPool()
    {
        if(availableAfterImages.Count == 0)
        {
            GrowPool();
        }

        var instance = availableAfterImages.Dequeue();
        instance.SetActive(true);
        return instance;
    }

    public void PlaceAfterImage(Transform player)
    {
        if (Mathf.Abs(player.position.x - lastAfterImageXPosition) > distanceBetweenAfterImages) // places dash after images
        {
            PlayerAfterImageObjectPool.Instance.GetFromPool();
            lastAfterImageXPosition = player.position.x;
        }
    }
}
