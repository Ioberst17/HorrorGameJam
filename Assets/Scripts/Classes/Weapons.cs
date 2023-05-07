using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapons : IDatabaseItem, IShoppable
{
    // Standard Information
    [SerializeField] private int _id;
    public int id { get { return _id; } set { _id = value; } }
    [SerializeField] private string _classType;
    public string classType { get { return _classType; } set { _classType = value; } }
    [SerializeField] private string _name;
    public string name { get { return _name; } set { _name = value; } }
    [SerializeField] private string _description;
    public string description { get { return _description; } set { _description = value; } }
    public int amount;
    public Sprite sprite;
    public string audioOnAcquisition;

    // Unique Information
    public int tier;
    public bool isSecondary, isShot, isThrown, isFixedDistance;
    public string statusModifier;
    public bool isKinetic, isElemental, isHeavy; // note: isHeavy is not for a weight attribute, but a weapon type attribute (kinetic, elemental, heavy)
    public string weight;
    public int level1Damage;
    public int level2Damage;
    public int level3Damage;
    public int level;
    public bool isLightSource;
    public float fireRate;
    public int ammoLimit;

    // Shop Information
    public bool isPurchasable;
    [SerializeField] private int _cost;
    public int cost { get { return _cost; } set { _cost = value; } }
    public int shopAmountPerPurchase;
    public int shopStock;

    public Weapons()
    {

    }

    public Weapons(int id, string classType, string name,  string description, int amount, Sprite sprite, string audioOnAcquisition, int tier, bool isSecondary, bool isShot, bool isThrown, bool isFixedDistance, string statusModifier, bool isKinetic, bool isElemental, bool isHeavy, string weight, int level1Damage, int level2Damage, int level3Damage, int level, bool isLightSource, float fireRate, int ammoLimit, bool isPurchasable, int cost, int shopAmountPerPurchase, int shopStock)
    {
        this.id = id;
        this.classType = classType;
        this.name = name;
        this.description = description;
        this.description = description;
        this.amount = amount;
        this.sprite = sprite;
        this.audioOnAcquisition = audioOnAcquisition;
        this.tier = tier;
        this.isSecondary = isSecondary;
        this.isShot = isShot;
        this.isThrown = isThrown;
        this.isFixedDistance = isFixedDistance;
        this.statusModifier = statusModifier;
        this.isKinetic = isKinetic;
        this.isElemental = isElemental;
        this.isHeavy = isHeavy;
        this.weight = weight;
        this.level1Damage = level1Damage;
        this.level2Damage = level2Damage;
        this.level3Damage = level3Damage;
        this.level = level;
        this.isLightSource = isLightSource;
        this.fireRate = fireRate;
        this.ammoLimit = ammoLimit;
        this.isPurchasable = isPurchasable;
        this.cost = cost;
        this.shopAmountPerPurchase = shopAmountPerPurchase;
        this.shopStock = shopStock;
    }

    public Weapons(Weapons input)
    {
        // Standard Information
     this.id = input.id;
     this.classType = input.classType;
     this.name = input.name;
     this.description = description;
     this.amount = input.amount;
     //this.sprite = Resources.Load<Sprite>("Sprites/Weapons" + name); ;
     this.audioOnAcquisition = input.audioOnAcquisition;

    // Unique Information
     this.tier = input.tier;
     this.isSecondary = input.isSecondary;
     this.isShot = input.isShot;
     this.isThrown = input.isThrown;
     this.isFixedDistance = input.isFixedDistance;
     this.statusModifier = input.statusModifier;
     this.isKinetic = input.isKinetic;
     this.isElemental = input.isElemental; 
     this.isHeavy = input.isHeavy; 
     this.weight = input.weight;
     this.level1Damage = input.level1Damage;
     this.level2Damage = input.level2Damage;
     this.level3Damage = input.level3Damage;
     this.level = input.level;
     this.isLightSource = input.isLightSource;
     this.fireRate = input.fireRate;
     this.ammoLimit = input.ammoLimit;

    // Shop Information
     this.isPurchasable = input.isPurchasable;
     this.cost = input.cost;
     this.shopAmountPerPurchase = input.shopAmountPerPurchase;
     this.shopStock = input.shopStock;
}
}
