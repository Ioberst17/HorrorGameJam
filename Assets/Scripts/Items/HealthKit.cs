using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthKit : Item
{
    private void Start()
    {
        Initialize();
        self.amount = 1;
        self.id = 1;
    }
}
