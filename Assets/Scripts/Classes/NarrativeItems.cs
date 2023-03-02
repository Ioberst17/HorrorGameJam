using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeItems
{
    public int id;
    public string itemType;
    public string itemName;
    public int amount;
    public string audioOnPickup;
    public string audioOnUse;
    public string description;
    public string flavorText;
    public Sprite sprite;

    public NarrativeItems()
    {

    }

    public NarrativeItems(NarrativeItems input)
    {
        this.id = input.id;
        this.itemType = input.itemType;
        this.itemName = input.itemName;
        this.amount = input.amount;
        this.audioOnPickup = input.audioOnPickup;
        this.audioOnUse = input.audioOnUse;
        this.description = input.description;
        sprite = Resources.Load<Sprite>("Sprites/" + itemName);
    }
}
