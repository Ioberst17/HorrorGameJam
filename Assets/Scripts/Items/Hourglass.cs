using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hourglass : Item
{
    private new void Start()
    {
        base.Initialize();
        self.amount = 1;
        self.id = 9;
    }
}
