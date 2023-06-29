using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImage : MonoBehaviour
{
    [SerializeField] private float activeTime = 0.3f;
    [SerializeField] public float timeActivated;
    [SerializeField] private float alpha;
    [SerializeField] private float alphaSet = 0.5f;
    [SerializeField] private float alphaMultiplier = 0.85f; // the smaller this number is, the faster the after images fade

    [SerializeField] public Vector3 ImagePlacement;

    private SpriteRenderer spriteRenderer;
    private PlayerAnimator playerAnimator;

    [SerializeField] private Color color;

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
        //player = GameObject.FindGameObjectWithTag("Player").transform;
        //playerAnimator = player.GetComponentInChildren<PlayerAnimator>();

        alpha = alphaSet;
        
        timeActivated = Time.time;
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            SetAlpha(alpha * alphaMultiplier);
        }
    }

    private void FixedUpdate()
    {
        if (Time.time >= (timeActivated + activeTime)) { PlayerAfterImageObjectPool.Instance.AddToPool(gameObject); }
    }

    public void SetAlpha(float alphaToApply)
    {
        alpha = alphaToApply;
        color = new Color(1f, 1f, 1f, alpha);
        if (spriteRenderer != null) { spriteRenderer.color = color; }
    }
}
