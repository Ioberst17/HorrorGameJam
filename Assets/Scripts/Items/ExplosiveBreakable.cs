using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Explode))]
public class ExplosiveBreakable : Breakable
{
    Explode explode;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        explode = GetComponent<Explode>();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    public override void HPZero()
    {
        base.HPZero();
        Instantiate(Resources.Load("VFXPrefabs/Explosion"), transform.position, Quaternion.identity);
        FindObjectOfType<AudioManager>().PlaySFX("WeaponExplosion");
        GetComponent<Explode>().ExplosionDamage(2f, 10f, 10);
    }
}
