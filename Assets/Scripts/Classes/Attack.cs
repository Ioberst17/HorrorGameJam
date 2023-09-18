using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack : IDatabaseItem
{
    // Standard Information
    [SerializeField] private int _id; public int id { get { return _id; } set { _id = value; } }
    public string owner;
    [SerializeField] private string _name; public string name { get { return _name; } set { _name = value; } }
    public string nameNoSpace;
    public string nameInEngine;

    public bool isKinetic;
    public bool isMelee;
    public bool isRanged;
    public bool isElemental;

    public float launchForceX;
    public float launchForceY;

    public string statusModifier;

    public int baseDamage;
    public int maxDamage;

    public float attackBuffer;
    public float startupTime;
    public float activeTime;
    public float recoveryTime;

    public float hitBoxPoint1X;
    public float hitBoxPoint1Y;
    public float hitBoxPoint2X;
    public float hitBoxPoint2Y;
    
    [SerializeField] private string _description; public string description { get { return _description; } set { _description = value; } }

    public Attack() { }

    public Attack(int id, string owner, string name, string nameNoSpace, string nameInEngine,
              bool isKinetic, bool isMelee, bool isRanged, bool isElemental,
              float launchForceX, float launchForceY, 
              string statusModifier,
              int baseDamage, int maxDamage,
              float startupTime, float recoveryTime, 
              float hitBoxPoint1X, float hitBoxPoint1Y,
              float hitBoxPoint2X, float hitBoxPoint2Y,
              string description)
    {
        this.id = id;
        this.owner = owner;
        this.name = name;
        this.nameNoSpace = nameNoSpace;
        this.nameInEngine = nameInEngine;
        this.isKinetic = isKinetic;
        this.isMelee = isMelee;
        this.isRanged = isRanged;
        this.isElemental = isElemental;
        this.launchForceX = launchForceX; 
        this.launchForceY = launchForceY;
        this.statusModifier = statusModifier;
        this.baseDamage = baseDamage;
        this.maxDamage = maxDamage;
        this.startupTime = startupTime; 
        this.recoveryTime = recoveryTime; 
        this.hitBoxPoint1X = hitBoxPoint1X;
        this.hitBoxPoint1Y = hitBoxPoint1Y;
        this.hitBoxPoint2X = hitBoxPoint2X;
        this.hitBoxPoint2Y = hitBoxPoint2Y;
        this.description = description;
    }

}
