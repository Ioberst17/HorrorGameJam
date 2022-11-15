using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapons 
{
    public int id;
    public string title;
    public int tier;
    public int price;
    public bool isShot, isThrown;
    public bool isKinetic, isElemental, isHeavy; // note: isHeavy is not for a weight attribute, but a weapon type attribute (kinetic, elemental, heavy)
    public string weight;
    public int level1Damage;
    public int level2Damage;
    public int level3Damage;
    public int level;
    public bool isLightSource;
    public string description;
    public int amount;
}
