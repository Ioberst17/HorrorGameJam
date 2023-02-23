using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : Item
{
    private new void Start()
    {
        base.Initialize();
        self.amount = 1;
        self.id = 8;
    }
}
