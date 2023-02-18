using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class PlayerConsumables
{
    public int id;
    public string itemType;
    public string itemName;
    public int amount;
    public string audioOnPickup;
    public string audioOnUse;
    public string description;

    [SerializeField]
    public PlayerConsumables(int id, string itemType, string itemName, int amount, string audioOnPickup, string audioOnUse, string description)
    {
        this.id = id;
        this.itemType = itemType;
        this.itemName = itemName;
        this.amount = amount;
        this.audioOnPickup = audioOnPickup;
        this.audioOnUse = audioOnUse;
        this.description = description;
    }
}
