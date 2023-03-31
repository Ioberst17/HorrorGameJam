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
        numOfColumns = 7; // must be updated as CSV is updated
        textAssetData = Resources.Load<TextAsset>("TextFiles/ConsumablesDatabase");
        string[] data = ReadCSV();
        CreateDatabase(data);
    }

    public string ReturnNameFromID(int id) 
    {
        string toReturn = "No Match"; Debug.Log("ReturnNameFromID in ConsumablesDatabase.cs returned no match");
        for(int i = 0; i < this.data.entries.Length; i++)
        {
            if (id == this.data.entries[i].id) { toReturn = this.data.entries[i].itemName; }
        }
        return toReturn;
    }

    public int ReturnIDfromName(string name)
    {
        int toReturn = -1; Debug.Log("ReturnNameFromID in ConsumablesDatabase.cs returned no match");
        for (int i = 0; i < this.data.entries.Length; i++)
        {
            if (name == this.data.entries[i].itemName) { toReturn = this.data.entries[i].id; }
        }
        return toReturn;
    }

}
