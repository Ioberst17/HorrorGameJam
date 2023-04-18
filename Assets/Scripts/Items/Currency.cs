using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Currency : Item
{
    private int id;
    private int amount;

    private new void Start()
    {
        base.Initialize();
        self.amount = 1;
        id = 0;
    }
}
