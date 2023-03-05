using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Breakable : MonoBehaviour, IDamageable
{
    private Animator anim;
    private int hitCount = 3;
    private SpriteRenderer spriteRenderer;
    public GameObject BrokenVersion;
    private bool hasBroken = false;
    public string breakableName;

    // Start is called before the first frame update
    void Start()
    {
        int BreakableEnviroLayer = LayerMask.NameToLayer("BreakableEnviro");
        gameObject.layer = BreakableEnviroLayer;
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (hitCount < 1 && hasBroken == false)
        {
            HPZero();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (/*collision.gameObject.GetComponent<PlayerController>() != null ||*/ collision.gameObject.GetComponent<Ammo>() != null)
        {
            if(hasBroken == false)
            {
                TakeDamage();
            }
        }
    }

    void TakeDamage()
    {
        hitCount += -1;
        anim.SetTrigger("Hit");
    }

    void HPZero()
    {
        spriteRenderer.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        Instantiate(BrokenVersion, transform.position, Quaternion.identity);
        if (GetComponent<BreakableLoot>() != null) { GetComponent<BreakableLoot>().GenerateLoot(); }
        hasBroken = true;
    }


}
