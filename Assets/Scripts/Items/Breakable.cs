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
    [SerializeField] public int hitCount = 3;
    [SerializeField] public bool needsToBeHitWithExplosion; 
    private SpriteRenderer spriteRenderer;
    public GameObject BrokenVersion;
    public bool hasBroken = false;
    private UIPulse pulse;

    // Start is called before the first frame update
    public void Start()
    {
        pulse = GetComponent<UIPulse>();
        gameObject.layer = LayerMask.NameToLayer("BreakableEnviro");
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Update() { if (hitCount < 1 && hasBroken == false) { HPZero();} }

    public void Hit() { if (!hasBroken && !needsToBeHitWithExplosion) { hitCount += -1; pulse.pulseTrigger = true; } }

    public void Hit(int Damage) { Hit(); } 

    public void Hit(int Damage, Vector3 playerPos) { Hit(); }

    public void Hit(GameObject isHitBy)
    {
        if (!hasBroken && needsToBeHitWithExplosion) { if (isHitBy.gameObject.GetComponent<Explode>() != null) { hitCount += -1; pulse.pulseTrigger = true; } }
        else { Hit(); }
    }

    public void Hit(int Damage, Vector3 playerPos, GameObject isHitBy) { Hit(isHitBy); }

    public virtual void HPZero()
    {
        spriteRenderer.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        if (BrokenVersion != null) { Instantiate(BrokenVersion, transform.position, Quaternion.identity); }
        else { Debug.LogFormat("A BrokenVersion prefab is not attached to {0} that just broke; if you want to create a broken version, add it to the inspector", gameObject.name); }
        if (GetComponent<BreakableLoot>() != null) { GetComponent<BreakableLoot>().GenerateLoot(); }
        hasBroken = true;
    }


}
