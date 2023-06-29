using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ComponentFinder;

public class PlayerAfterImageObjectPool : ObjectPool
{
    public float distanceBetweenAfterImages;
    private float lastAfterImageXPosition;
    PlayerController playerController;

    SpriteRenderer spriteRendererRightArm, spriteRendererLeftArm, spriteRendererBase;
    SpriteRenderer[] bodyPartSpriteRenderers;

    public static PlayerAfterImageObjectPool Instance;

    public override void Awake()
    {
        Instance = this;
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene currentScene, LoadSceneMode mode)
    {
        playerController = FindObjectOfType<PlayerController>();
        spriteRendererRightArm = GetComponentInChildrenByNameAndType<RightArmAnimator>("SpriteAndAnimations", playerController.gameObject).GetComponent<SpriteRenderer>();
        spriteRendererLeftArm = GetComponentInChildrenByNameAndType<LeftArmAnimator>("SpriteAndAnimations", playerController.gameObject).GetComponent<SpriteRenderer>();
        spriteRendererBase = GetComponentInChildrenByNameAndType<BaseAnimator>("SpriteAndAnimations", playerController.gameObject).GetComponent<SpriteRenderer>();
        bodyPartSpriteRenderers = new SpriteRenderer[] { spriteRendererRightArm, spriteRendererLeftArm, spriteRendererBase };
    }

    public void PlaceAfterImage(Transform player)
    {
        if (Mathf.Abs(player.position.x - lastAfterImageXPosition) > distanceBetweenAfterImages) // places dash after images
        {
            Instance.GetFromPool();
            lastAfterImageXPosition = player.position.x;
        }
    }

    override public GameObject GetFromPool()
    {
        if (availablePrefabs.Count < 3) { GrowPool(); }
        foreach (SpriteRenderer renderer in bodyPartSpriteRenderers)
        {
            var instance = availablePrefabs.Dequeue();
            // place local position at zero (since it is a child of player controller)
            instance.transform.localPosition = Vector3.zero;
            instance.GetComponent<PlayerAfterImage>().ImagePlacement = instance.transform.position;

            // set sprite to current sprite from renderer
            instance.GetComponent<SpriteRenderer>().sprite = renderer.sprite;

            // grow scale to match default sprite size
            instance.transform.localScale = new Vector3(3, 3, 1);

            // match direction
            if(playerController.FacingDirection == -1) { instance.transform.rotation = Quaternion.Euler(0, -180, 0); } // if facing left, do this
            else { instance.transform.rotation = Quaternion.Euler(0, 0, 0); } // if facing right, do this

            instance.SetActive(true);
        }

        return null;
    }

}
