using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAfterImageObjectPool : ObjectPool
{
    public float distanceBetweenAfterImages;
    private float lastAfterImageXPosition;

    SpriteRenderer spriteRendererRightArm, spriteRendererLeftArm, spriteRendererBase;
    SpriteRenderer[] bodyPartSpriteRenderers;

    public static PlayerAfterImageObjectPool Instance;

    public override void Awake()
    {
        Instance = this;
        GrowPool();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene currentScene, LoadSceneMode mode)
    {
        spriteRendererRightArm = FindObjectOfType<RightArmAnimator>().GetComponent<SpriteRenderer>();
        spriteRendererLeftArm = FindObjectOfType<LeftArmAnimator>().GetComponent<SpriteRenderer>();
        spriteRendererBase = FindObjectOfType<BaseAnimator>().GetComponent<SpriteRenderer>();
        bodyPartSpriteRenderers = new SpriteRenderer[] { spriteRendererRightArm, spriteRendererLeftArm, spriteRendererBase };
    }

    public void PlaceAfterImage(Transform player)
    {
        if(availablePrefabs.Count >= 3) { GrowPool(); }
        if (Mathf.Abs(player.position.x - lastAfterImageXPosition) > distanceBetweenAfterImages) // places dash after images
        {
            Instance.GetFromPool();
            lastAfterImageXPosition = player.position.x;
        }
    }

    override public GameObject GetFromPool()
    {
        if (availablePrefabs.Count >= 3)
        {
            GrowPool();
        }

        foreach(SpriteRenderer renderer in bodyPartSpriteRenderers)
        {
            var instance = availablePrefabs.Dequeue();
            instance.SetActive(true);
            instance.GetComponent<SpriteRenderer>().sprite = renderer.sprite;
            instance.transform.localScale = new Vector3(3, 3, 1);
        }

        return null;
    }

}
