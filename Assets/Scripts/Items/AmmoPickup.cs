using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : Item
{
    public int itemID;
    public int amount;
    private new void Start()
    {
        Initialize();
        self.id = itemID;
        self.amount = amount;
    }


}
