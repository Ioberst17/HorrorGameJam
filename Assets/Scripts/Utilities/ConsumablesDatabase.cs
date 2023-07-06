using Ink.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ConsumablesDatabase : Database<Consumables>
{
    // must be attached to a game object in the scene hierarchy
    // creates a consumables database (the store of weapons and information about them), and reads it from the consumablesDatbase.csv in /Resources

    private void Awake()
    {
        numOfColumns = 14; // must be updated as CSV is updated
        textAssetData = Resources.Load<TextAsset>("TextFiles/ConsumablesDatabase");
        string[] data = ReadCSV();
        CreateDatabase(data);
    }
}
