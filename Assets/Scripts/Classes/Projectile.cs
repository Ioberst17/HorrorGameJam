using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Projectile : IDatabaseItem
{
    // Standard Information
    [SerializeField] private int _id; public int id { get { return _id; } set { _id = value; } }
    public int referenceID;
    public string owner;
    public string ownerNoSpace;
    [SerializeField] private string _name; public string name { get { return _name; } set { _name = value; } }
    public string nameNoSpace;

    public string audioOnUse, audioOnHit;

    public bool isShot, isThrown, isExplosive, isFixedDistance, isTargeted;

    // used by projectiles that must complete a cycle e.g. targeted spells that will play through the animation
    public bool playTillEnd;

    public string statusModifier;

    public bool isKinetic, isElemental, isHeavy;

    public int baseDamage, maxDamage;

    public float firePointX, firePointY;

    public float fireRate;
    public int fireRateFrames;

    // force is used if adding force, speed is used if launching at velocity
    public float launchForceX, launchForceY, launchSpeedX, launchSpeedY;
    
    public float startingGravityScale;

    public float targetingModifierX, targetingModifierY;

    public Projectile() { }

    public Projectile(int id, int referenceID, 
                  string owner, string ownerNoSpace, string name, string nameNoSpace, 
                  string audioOnUse, string audioOnHit, 
                  bool isShot, bool isThrown, bool isFixedDistance, bool isTargeted,
                  bool playTillEnd, string statusModifier, 
                  bool isKinetic, bool isElemental, bool isHeavy,
                  int baseDamage, int maxDamage, 
                  float firePointX, float firePointY, float fireRate, int fireRateFrames,
                  float launchForceX, float launchForceY, float launchSpeedX, float launchSpeedY,
                  float startingGravityScale,
                  float targetingModifierX, float targetingModifierY)
    {
        // ids
        this.id = id;
        this.referenceID = referenceID;
        // owners / names
        this.owner = owner;
        this.ownerNoSpace = ownerNoSpace;
        this.name = name;
        this.nameNoSpace = nameNoSpace;
        // audio
        this.audioOnUse = audioOnUse;
        this.audioOnHit = audioOnHit;
        // characteristics
        this.isShot = isShot;
        this.isThrown = isThrown;
        this.isFixedDistance = isFixedDistance;
        this.isTargeted = isTargeted;
        this.playTillEnd = playTillEnd;
        this.statusModifier = statusModifier;
        this.isKinetic = isKinetic;
        this.isElemental = isElemental;
        this.isHeavy = isHeavy;
        // damage
        this.baseDamage = baseDamage;
        this.maxDamage = maxDamage;
        // shooting modifiers
        // firing
        this.firePointX = firePointX;
        this.firePointY = firePointY;
        this.fireRate = fireRate;
        this.fireRateFrames = fireRateFrames;
        // launching
        this.launchForceX = launchForceX;
        this.launchForceY = launchForceY;
        this.launchSpeedX = launchSpeedX;
        this.launchSpeedY = launchSpeedY;
        //gravity
        this.startingGravityScale = startingGravityScale;
        // targeting
        this.targetingModifierX = targetingModifierX;
        this.targetingModifierY = targetingModifierY;
    }
}
