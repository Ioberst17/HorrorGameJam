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
}
