using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponDatabase : MonoBehaviour
{
    // must be attached to a game object in the scene hierarchy
    // creates a weapons database (the store of weapons and information about them), and reads it from the itemDatbase.csv in /Resources

    private TextAsset textAssetData; // the CSV to read from, must be assigned in Inspector

    [System.Serializable]
    public class Database // create the a database of all game items
    {
        public Weapons[] entries;
    }

    public Database weaponDatabase = new Database();

    private void Awake() // load CSV from Resources folder then add it to the Weapon Database game object
    {
        textAssetData = Resources.Load<TextAsset>("TextFiles/WeaponDatabase"); // loads Weapon Database CSV into this variable
        ReadCSV();
    }

    void ReadCSV() // adds database to the weaponDatabase game object that should be in scene, from a csv file
    {
        string[] data = textAssetData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

        int numOfColumns = 17; // Update as columns are added

        int tableSize = data.Length / numOfColumns - 1; // gets data length (total # of cells), then divides by # of columns to get # of entries, then -1 for ignoring the header row
        weaponDatabase.entries = new Weapons[tableSize];
        Debug.Log(tableSize);

        for (int i = 0; i < tableSize; i++)
        {
            weaponDatabase.entries[i] = new Weapons(); // creates new entry in memory

            // adds row data to the i-th entry of the database to database
            weaponDatabase.entries[i].id = int.Parse(data[numOfColumns * (i + 1)]);
            weaponDatabase.entries[i].title = data[numOfColumns * (i + 1) + 1];
            weaponDatabase.entries[i].tier = int.Parse(data[numOfColumns * (i + 1) + 2]);
            weaponDatabase.entries[i].price = int.Parse(data[numOfColumns * (i + 1) + 3]);
            weaponDatabase.entries[i].isShot = bool.Parse(data[numOfColumns * (i + 1) + 4]);
            weaponDatabase.entries[i].isThrown = bool.Parse(data[numOfColumns * (i + 1) + 5]);
            weaponDatabase.entries[i].isKinetic = bool.Parse(data[numOfColumns * (i + 1) + 6]);
            weaponDatabase.entries[i].isElemental = bool.Parse(data[numOfColumns * (i + 1) + 7]);
            weaponDatabase.entries[i].isHeavy = bool.Parse(data[numOfColumns * (i + 1) + 8]);
            weaponDatabase.entries[i].weight = data[numOfColumns * (i + 1) + 9];
            weaponDatabase.entries[i].level1Damage = int.Parse(data[numOfColumns * (i + 1) + 10]);
            weaponDatabase.entries[i].level2Damage = int.Parse(data[numOfColumns * (i + 1) + 11]);
            weaponDatabase.entries[i].level3Damage = int.Parse(data[numOfColumns * (i + 1) + 12]);
            weaponDatabase.entries[i].level = int.Parse(data[numOfColumns * (i + 1) + 13]);
            weaponDatabase.entries[i].isLightSource = bool.Parse(data[numOfColumns * (i + 1) + 14]);
            weaponDatabase.entries[i].description = data[numOfColumns * (i + 1) + 15];
            weaponDatabase.entries[i].amount = int.Parse(data[numOfColumns * (i + 1) + 16]);
}
    }

}
