using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NarrativeItems : IDatabaseItem, IShoppable
{
    // Standard information
    [SerializeField] private int _id;
    public int id { get { return _id; } set { _id = value; } }
    [SerializeField] private string _classType;
    public string classType { get { return _classType; } set { _classType = value; } }
    [SerializeField] private string _name;
    public string name { get { return _name; } set { _name = value; } }
    [SerializeField] private string _description;
    public string description { get { return _description; } set { _description = value; } }
    public float amount;
    public string audioOnAcquisition;
    public Sprite sprite;

    // Unique Information
    public string stat;
    public string modifier;

    // Shop information
    public bool isPurchasable;
    [SerializeField] private int _cost;
    public int cost { get { return _cost; } set { _cost = value; } }
    public int shopAmountPerPurchase;
    public int shopStock;
    

    public NarrativeItems()
    {

    }

    public NarrativeItems(NarrativeItems input)
    {
        this.id = input.id;
        this.classType = input.classType;
        this.name = input.name;
        this.stat = input.stat;
        this.modifier = input.modifier;
        this.amount = input.amount;
        this.audioOnAcquisition = input.audioOnAcquisition;
        this.description = input.description;
        this.cost = input.cost;
        this.shopAmountPerPurchase = input.shopAmountPerPurchase;
        this.sprite = Resources.Load<Sprite>("Sprites/" + name);
    }
}
