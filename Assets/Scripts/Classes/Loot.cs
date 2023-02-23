using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Loot
{
    public Sprite lootSprite;
    public string lootName;
    public int dropChance;
    public int amount;

    public Loot (string lootName, int dropChance, int amount)
    {
        this.lootName = lootName;
        this.dropChance = dropChance;
        this.amount = amount;
    }
}
