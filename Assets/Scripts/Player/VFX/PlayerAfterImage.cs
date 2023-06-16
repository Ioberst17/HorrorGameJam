using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImage : MonoBehaviour
{
    [SerializeField]
    private float activeTime = 0.1f;
    private float timeActivated;
    private float alpha;
    [SerializeField]
    private float alphaSet = 0.5f;
    private float alphaMultiplier = 0.85f; // the smaller this number is, the faster the after images fade

    [SerializeField]
    private Vector3 ImagePlacement;
    [SerializeField]
    private Transform player;

    private SpriteRenderer spriteRenderer;
    private PlayerAnimator playerAnimator;

    private Color color;

    public void LateUpdate()
    {
        if (gameObject.activeSelf)
        {
            transform.position = ImagePlacement;
        }
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerAnimator = player.GetComponentInChildren<PlayerAnimator>();

        alpha = alphaSet;
        spriteRenderer.sprite = playerAnimator.ReturnPlayerImage();
        ImagePlacement = player.position;
        timeActivated = Time.time;
    }

    private void Update()
    {
        alpha = alphaMultiplier;
        color = new Color(1f, 1f, 1f, alpha);
        spriteRenderer.color = color;
    }

    private void FixedUpdate()
    {
        if (Time.time >= (timeActivated + activeTime)) { PlayerAfterImageObjectPool.Instance.AddToPool(gameObject); }
    }
}
