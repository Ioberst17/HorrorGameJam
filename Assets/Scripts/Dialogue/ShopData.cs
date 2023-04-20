using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base item class
public class ShopItem
{
    public int ID;
    public string name;
    public int cost;
    public int amount;

    public ShopItem(int id, string name, int cost, int amount)
    {
        this.ID = id;
        this.name = name;
        this.cost = cost;
        this.amount = amount;
    }
}

// Subclass for narrative items
public class ShopNarrativeItem : ShopItem
{
    // Add any additional properties unique to narrative items

    public ShopNarrativeItem(int id, string name, int cost, int amount) : base(id, name, cost, amount)
    {
        // Any additional initialization code goes here
    }
}

// Subclass for consumables
public class ShopConsumable : ShopItem
{
    // Add any additional properties unique to consumables

    public ShopConsumable(int id, string name, int cost, int amount) : base(id, name, cost, amount)
    {
        // Any additional initialization code goes here
    }
}

public class ShopConsumableData
{
    public static ShopConsumable HealthKit = new ShopConsumable(0, "Health Kit", 10, 1);
}

// Subclass for secondary weapons
public class ShopSecondaryWeapon : ShopItem
{
    // Add any additional properties unique to secondary weapons
    public ShopSecondaryWeapon(int id, string name, int cost, int amount) : base(id, name, cost, amount)
    {
        // Any additional initialization code goes here
    }
}

// Class for storing shop data
public class ShopData
{
    public List<ShopNarrativeItem> narrativeItems;
    public List<ShopConsumable> consumables;
    public List<ShopSecondaryWeapon> secondaryWeapons;

    public ShopData()
    {
        narrativeItems = new List<ShopNarrativeItem>();
        consumables = new List<ShopConsumable>();
        secondaryWeapons = new List<ShopSecondaryWeapon>();
    }
}
