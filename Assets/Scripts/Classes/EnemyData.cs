using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyData : IDatabaseItem // are added in the same order as the column schema
{
    // CORE ATTRIBUTES
    [SerializeField] private int _id;
    public int id { get { return _id; } set { _id = value; } }
    [SerializeField] private string _name;
    public string name { get { return _name; } set { _name = value; } }
    public string nameNoSpace;
    public int tier;
    public bool isFlying;
    public bool isGround;
    public int health;
    public int knockback;
    public float movementSpeed;
    public int soulPointsDropped;

    // DIFFICULTY INFO
    public float easyHPMultiplier, mediumHPMultiplier, hardHPMultiplier;
    public float easyAPMultiplier, mediumAPMultiplier, hardAPMultiplier;    

    //ATTACK INFO
    public string attack1;
    public bool attack1Kinetic,attack1Melee, attack1Ranged, attack1Elemental;
    public int attack1Damage;
    public string attack2;
    public bool attack2Kinetic, attack2Melee, attack2Ranged, attack2Elemental;
    public int attack2Damage;
    public string attack3;
    public bool attack3Kinetic, attack3Melee, attack3Ranged, attack3Elemental;
    public int attack3Damage;
    public string attack4;
    public bool attack4Kinetic, attack4Melee, attack4Ranged, attack4Elemental;
    public int attack4Damage;
    public string attack5;
    public bool attack5Kinetic, attack5Melee, attack5Ranged, attack5Elemental;
    public int attack5Damage;
    public string attack6;
    public bool attack6Kinetic, attack6Melee, attack6Ranged, attack6Elemental;
    public int attack6Damage;

    // OTHER
    public string description;

    // LOOT DROP
    public string loot1name;
    public int loot1dropChance;
    public int loot1amount;
    public string loot2name;
    public int loot2dropChance;
    public int loot2amount;
    public string loot3name;
    public int loot3dropChance;
    public int loot3amount;


}
