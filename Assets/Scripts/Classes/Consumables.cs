using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Consumables
{
    public int id;
    public string itemType;
    public string itemName;
    public int amount;
    public string audioOnPickup;
    public string audioOnUse;
    public string description;
}
