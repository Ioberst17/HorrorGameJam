using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(UIPulse))]
public class Breakable : MonoBehaviour, IDamageable
{
    private int hitCount = 3;
    private SpriteRenderer spriteRenderer;
    public GameObject BrokenVersion;
    private bool hasBroken = false;
    private UIPulse pulse;

    // Start is called before the first frame update
    void Start()
    {
        pulse = GetComponent<UIPulse>();
        gameObject.layer = LayerMask.NameToLayer("BreakableEnviro");
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update() { if (hitCount < 1 && hasBroken == false) { HPZero();} }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (/*collision.gameObject.GetComponent<PlayerController>() != null ||*/ collision.gameObject.GetComponent<Ammo>() != null)
        {
            if (hasBroken == false) { Hit();}
        }
    }

    public void Hit() { hitCount += -1; pulse.pulseTrigger = true; }

    public void Hit(int Damage) { Hit(); } 

    public void Hit(int Damage, Vector3 playerPos) { Hit(); }

    void HPZero()
    {
        spriteRenderer.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        Instantiate(BrokenVersion, transform.position, Quaternion.identity);
        if (GetComponent<BreakableLoot>() != null) { GetComponent<BreakableLoot>().GenerateLoot(); }
        hasBroken = true;
    }


}
