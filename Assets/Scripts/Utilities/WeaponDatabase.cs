using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using Ink.Parsed;
using Ink.Runtime;
using JetBrains.Annotations;

public class WeaponDatabase : MonoBehaviour
{
    // must be attached to a game object in the scene hierarchy
    // creates a weapons database (the store of weapons and information about them), and reads it from the weaponDatbase.csv in /Resources

    private TextAsset textAssetData; // the CSV to read from, must be assigned in Inspector

    [Serializable]
    public class Database // create the a database of all game items
    {
        public Weapons[] entries;
    }

    public Database weaponDatabase = new Database();
    [SerializeField]
    private List<string> columnNames;
    [SerializeField]
    private List<string> columnDataTypes;
    public List<int> validPrimaryWeaponIDs;
    public List<int> validSecondaryWeaponIDs;
    private int numOfColumns = 20; // must be updated as CSV is updated

    private void Awake() 
    {
        textAssetData = Resources.Load<TextAsset>("TextFiles/WeaponDatabase"); 
        string[] data = ReadCSV();
        CreateDatabase(data);

        validPrimaryWeaponIDs = CreateListOfValidWeapons("Primary");
        validSecondaryWeaponIDs = CreateListOfValidWeapons("Secondary");
    }

    string[] ReadCSV()
    {
        string[] data = textAssetData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        return data;
    }

    void CreateDatabase(string[] data)
    {
        int numOfRows = data.Length / numOfColumns -2; // gets data length (total # of cells), then divides by # of columns to get # of rowsl -2 for header and datatype row
        weaponDatabase.entries = new Weapons[numOfRows];

        columnNames = GetColumnData(data, "columnNames");
        columnDataTypes = GetColumnData(data, "variableDataTypes");

        for (int i = 0; i < numOfRows; i++) 
        {
            weaponDatabase.entries[i] = new Weapons();

            FieldInfo[] columnsToFill = weaponDatabase.entries[i].GetType().GetFields();

            for (int j = 0; j < columnsToFill.Length; j++)
            {
                foreach (var dataEntry in columnsToFill)
                {
                    if (dataEntry.Name == columnNames[j])
                    {
                        ParseDataToTable(i, j, data, dataEntry);
                    }
                }

                /*weaponDatabase.entries[i].id = int.Parse(data[numOfColumns * (i + 1)]);
                weaponDatabase.entries[i].title = data[numOfColumns * (i + 1) + 1];
                weaponDatabase.entries[i].tier = int.Parse(data[numOfColumns * (i + 1) + 2]);
                weaponDatabase.entries[i].price = int.Parse(data[numOfColumns * (i + 1) + 3]);
                weaponDatabase.entries[i].isSecondary = bool.Parse(data[numOfColumns * (i + 1) + 4]);
                weaponDatabase.entries[i].isShot = bool.Parse(data[numOfColumns * (i + 1) + 5]);
                weaponDatabase.entries[i].isThrown = bool.Parse(data[numOfColumns * (i + 1) + 6]);
                weaponDatabase.entries[i].isFixedDistance = bool.Parse(data[numOfColumns * (i + 1) + 7]);
                weaponDatabase.entries[i].isKinetic = bool.Parse(data[numOfColumns * (i + 1) + 8]);
                weaponDatabase.entries[i].isElemental = bool.Parse(data[numOfColumns * (i + 1) + 9]);
                weaponDatabase.entries[i].isHeavy = bool.Parse(data[numOfColumns * (i + 1) + 10]);
                weaponDatabase.entries[i].weight = data[numOfColumns * (i + 1) + 11];
                weaponDatabase.entries[i].level1Damage = int.Parse(data[numOfColumns * (i + 1) + 12]);
                weaponDatabase.entries[i].level2Damage = int.Parse(data[numOfColumns * (i + 1) + 13]);
                weaponDatabase.entries[i].level3Damage = int.Parse(data[numOfColumns * (i + 1) + 14]);
                weaponDatabase.entries[i].level = int.Parse(data[numOfColumns * (i + 1) + 15]);
                weaponDatabase.entries[i].isLightSource = bool.Parse(data[numOfColumns * (i + 1) + 16]);
                weaponDatabase.entries[i].fireRate = float.Parse(data[numOfColumns * (i + 1) + 17]);
                weaponDatabase.entries[i].description = data[numOfColumns * (i + 1) + 18];
                weaponDatabase.entries[i].amount = int.Parse(data[numOfColumns * (i + 1) + 19]);*/
            }
        }
    }

    void ParseDataToTable(int currentRow, int currentColumnEntry, string[] data, FieldInfo dataEntry)
    {
        int rowsToSkip = 2; //header row and datatype row are skipped when parsing

        if (columnDataTypes[currentColumnEntry] == "int")
        {
            int intVal = int.Parse(data[numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry]);
            weaponDatabase.entries[currentRow].GetType().GetField(dataEntry.Name).SetValue(weaponDatabase.entries[currentRow], intVal);
        }
        else if (columnDataTypes[currentColumnEntry] == "string")
        {
            string strVal = data[numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry];
            weaponDatabase.entries[currentRow].GetType().GetField(dataEntry.Name).SetValue(weaponDatabase.entries[currentRow], strVal);
        }
        else if (columnDataTypes[currentColumnEntry] == "bool")
        {
            bool boolVal = bool.Parse(data[numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry]);
            weaponDatabase.entries[currentRow].GetType().GetField(dataEntry.Name).SetValue(weaponDatabase.entries[currentRow], boolVal);
        }
        else if (columnDataTypes[currentColumnEntry] == "float")
        {
            float floatVal = float.Parse(data[numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry]);
            weaponDatabase.entries[currentRow].GetType().GetField(dataEntry.Name).SetValue(weaponDatabase.entries[currentRow], floatVal);
        }
        else { Debug.Log("No data type match. Current column is " + columnNames[currentColumnEntry] + " with data type " + columnDataTypes[currentColumnEntry]); }
    }

    // SUPPORT FUNCTIONS
    private List<string> GetColumnData(string[] data, string typeOfList)
    {
        List<string> toReturn = new List<string>();

        weaponDatabase.entries[0] = new Weapons();

        FieldInfo[] fieldsToLoop = weaponDatabase.entries[0].GetType().GetFields();

        for (int i = 0; i < fieldsToLoop.Length; i++)
        {
            string nameOfField = fieldsToLoop[i].Name;
            var dataType = data[numOfColumns * (0 + 1) + i];

            if (typeOfList == "columnNames"){ toReturn.Add(nameOfField); }
            else if (typeOfList == "variableDataTypes") { toReturn.Add(dataType); }
            else { Debug.Log("Check for typo on string 'typeOfList' input"); }
        }
        return toReturn;
    }
    

    private List<int> CreateListOfValidWeapons(string PrimaryOrSecondary)
    {
        List<int> checker = new List<int>();

        for (int i = 0; i < weaponDatabase.entries.Length; i++)
        {
            if (PrimaryOrSecondary == "Secondary")
            {
                if (weaponDatabase.entries[i].isSecondary) { checker.Add(weaponDatabase.entries[i].id); }
            }
            else if (PrimaryOrSecondary == "Primary")
            {
                if (!weaponDatabase.entries[i].isSecondary) { checker.Add(weaponDatabase.entries[i].id); }
            }
        }

        return checker;
    }
}
