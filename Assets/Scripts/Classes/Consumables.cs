using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Consumables
{
    public string itemType;
    public string name;
    public int id;
    public int amount;
    public string description;
    public List<Consumables> consumablesList;
    public Consumables(string itemType, string name, int id, int amount, string description)
    {
        this.itemType = itemType;
        this.name = name;
        this.id = id;
        this.amount = amount;
        this.description = description;
    }

    public Consumables()
    {
       consumablesList = new List<Consumables>();
    }
    
}
