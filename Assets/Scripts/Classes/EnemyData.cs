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
