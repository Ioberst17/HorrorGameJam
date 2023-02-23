using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImageObjectPool : ObjectPool
{
    public float distanceBetweenAfterImages;
    private float lastAfterImageXPosition;

    public static PlayerAfterImageObjectPool Instance;

    public override void Awake()
    {
        Instance = this;
        GrowPool();
    }

    public void PlaceAfterImage(Transform player)
    {
        if (Mathf.Abs(player.position.x - lastAfterImageXPosition) > distanceBetweenAfterImages) // places dash after images
        {
            Instance.GetFromPool();
            lastAfterImageXPosition = player.position.x;
        }
    }

}
