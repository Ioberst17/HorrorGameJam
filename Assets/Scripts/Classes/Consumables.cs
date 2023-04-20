using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Consumables : IDatabaseItem, IShoppable
{
    // Standard Info
    [SerializeField] private int _id;
    public int id { get { return _id; } set { _id = value; } }
    [SerializeField] private string _classType;
    public string classType { get { return _classType; } set { _classType = value; } }
    [SerializeField] private string _name;
    public string name { get { return _name; } set { _name = value; } }
    [SerializeField] private string _description;
    public string description { get { return _description; } set { _description = value; } }
    public string itemType;
    public int amount;
    public string audioOnPickup;
    public string audioOnUse;
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
