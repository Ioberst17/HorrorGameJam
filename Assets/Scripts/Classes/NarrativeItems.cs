using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NarrativeItems : IShoppable
{
    // Standard information
    public int id;
    public string classType;
    public string name;
    public float amount;
    public string audioOnAcquisition;
    public string description;
    public Sprite sprite;

    // Unique Information
    public string stat;
    public string modifier;

    // Shop information
    public int isPurchasable;
    [SerializeField] public int cost { get; set; }
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
