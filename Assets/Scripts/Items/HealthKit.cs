using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthKit : Item
{
    private int id = 1;
    private int amount = 1;

    private new void Start()
    {
        base.Initialize();
        self.amount = 1;
        self.id = 1;
    }
}
