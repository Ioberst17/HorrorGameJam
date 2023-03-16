using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using Ink.Parsed;
using Ink.Runtime;
using JetBrains.Annotations;
using System.Linq;
using Unity.VisualScripting;

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

    private Weapons weaponChecker;

    public Database weaponDatabase = new Database();
    [SerializeField]
    private List<string> columnNames;
    [SerializeField]
    private List<string> columnDataTypes;
    public List<int> validPrimaryWeaponIDs;
    public List<int> validSecondaryWeaponIDs;
    private int numOfColumns = 21; // must be updated as CSV is updated

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
        int numOfRows = data.Length / numOfColumns - 2; // gets data length (total # of cells), then divides by # of columns to get # of rowsl -2 for header and datatype row
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
                    if (dataEntry.Name == columnNames[j]) { ParseDataToTable(i, j, data, dataEntry); }
                }
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

            if (typeOfList == "columnNames") { toReturn.Add(nameOfField); }
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

    public int GetWeaponDamage(int weaponID, int LevelOfWeapon)
    {
        var ammoLevel = LevelOfWeapon - 1;

        if (ammoLevel == 0) { return weaponDatabase.entries[weaponID].level1Damage; }
        else if (ammoLevel == 1) { return weaponDatabase.entries[weaponID].level2Damage; }
        else if (ammoLevel == 2) { return weaponDatabase.entries[weaponID].level3Damage; }
        else { Debug.LogFormat("Check to see if the correct weaponID and a Level of Weapon are being inputted to this function"); return -1; }
    }

    public string GetWeaponEffect(int weaponID)
    {
        weaponChecker = Array.Find(weaponDatabase.entries, x => x.id == weaponID);
        if (weaponChecker != null) { return weaponChecker.statusModifier; }

        Debug.Log("Weapon ID could not be found in weapon database; please check input");
        return "Error";
    }
}