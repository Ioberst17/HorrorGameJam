using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapons : IShoppable
{
    // Standard Information
    public int id;
    public string classType;
    public string name;
    public string description;
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
    public int cost { get; set; }
    public int shopAmountPerPurchase;
    public int shopStock;
}
