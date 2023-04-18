using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Consumables : IShoppable
{
    // Standard Info
    public int id;
    public string classType;
    public string itemType;
    public string name;
    public int amount;
    public string audioOnPickup;
    public string audioOnUse;
    public string description;
    public Sprite sprite;

    // Unique Info
    public int ammoID;

    // Shop Info
    public bool isPurchasable;
    [SerializeField] private int _cost;
    public int cost { get { return _cost; } set { _cost = value; } }
    public int shopAmountPerPurchase;
    public int shopStock;
    

    public Consumables()
    {

    }

    public Consumables(Consumables input)
    {
        this.id = input.id;
        this.classType = input.classType;
        this.itemType = input.itemType;
        this.name = input.name;
        this.amount = input.amount;
        this.audioOnPickup = input.audioOnPickup;
        this.audioOnUse = input.audioOnUse;
        this.description = input.description;
        this.ammoID = input.ammoID;
        sprite = Resources.Load<Sprite>("Sprites/" + name);
    }
}
