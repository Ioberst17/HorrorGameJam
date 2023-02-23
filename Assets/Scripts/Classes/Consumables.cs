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
    public int ammoID;
    public Sprite sprite;

    public Consumables()
    {

    }

    public Consumables(Consumables input)
    {
        this.id = input.id;
        this.itemType = input.itemType;
        this.itemName = input.itemName;
        this.amount = input.amount;
        this.audioOnPickup = input.audioOnPickup;
        this.audioOnUse = input.audioOnUse;
        this.description = input.description;
        this.ammoID = input.ammoID;
        sprite = Resources.Load<Sprite>("Sprites/" + itemName);
    }
}
