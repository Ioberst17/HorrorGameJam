using Ink.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ConsumablesDatabase : MonoBehaviour
{
    // must be attached to a game object in the scene hierarchy
    // creates a weapons database (the store of weapons and information about them), and reads it from the consumablesDatbase.csv in /Resources

    private TextAsset textAssetData; // the CSV to read from, must be assigned in Inspector

    [Serializable]
    public class Database // create the a database of all game items
    {
        public Consumables[] entries;
    }

    [SerializeField]
    public Database consumablesDatabase = new Database();
    [SerializeField]
    private List<string> columnNames;
    [SerializeField]
    private List<string> columnDataTypes;
    private int numOfColumns = 7; // must be updated as CSV is updated

    private void Awake()
    {
        textAssetData = Resources.Load<TextAsset>("TextFiles/ConsumablesDatabase");
        string[] data = ReadCSV();
        CreateDatabase(data);
    }

    string[] ReadCSV()
    {
        string[] data = textAssetData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        return data;
    }

    void CreateDatabase(string[] data)
    {
        int numOfRows = data.Length / numOfColumns - 2; // gets data length (total # of cells), then divides by # of columns to get # of rowsl -2 for header and datatype row
        consumablesDatabase.entries = new Consumables[numOfRows];

        columnNames = GetColumnData(data, "columnNames");
        columnDataTypes = GetColumnData(data, "variableDataTypes");

        for (int i = 0; i < numOfRows; i++)
        {
            consumablesDatabase.entries[i] = new Consumables();

            FieldInfo[] columnsToFill = consumablesDatabase.entries[i].GetType().GetFields();

            for (int j = 0; j < columnsToFill.Length; j++) 
            {
                foreach (var dataEntry in columnsToFill)
                {
                    if (dataEntry.Name == columnNames[j])
                    {
                        //Debug.Log("Current column being filled is " + dataEntry.Name + " and its type is " + columnDataTypes[j]);
                        ParseDataToTable(i, j, data, dataEntry);
                    }
                }
            }
        }
    }

    void ParseDataToTable(int currentRow, int currentColumnEntry, string[] data, FieldInfo dataEntry)
    {
        int rowsToSkip = 2; //header row and datatype row are skipped when parsing

        if (columnDataTypes[currentColumnEntry] == "int")
        {
            int.TryParse(data[numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry], out int intVal);
            consumablesDatabase.entries[currentRow].GetType().GetField(dataEntry.Name).SetValue(consumablesDatabase.entries[currentRow], intVal);
            //Debug.Log("Current item being added to database is " + consumablesDatabase.entries[currentRow].itemName + ". It's value is " + intVal);
        }
        else if (columnDataTypes[currentColumnEntry] == "string")
        {
            string strVal = data[numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry];
            consumablesDatabase.entries[currentRow].GetType().GetField(dataEntry.Name).SetValue(consumablesDatabase.entries[currentRow], strVal);
            //Debug.Log("Current item being added to database is " + consumablesDatabase.entries[currentRow].itemName + ". It's value is " + strVal);
        }
        else if (columnDataTypes[currentColumnEntry] == "bool")
        {
            bool boolVal = bool.Parse(data[numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry]);
            consumablesDatabase.entries[currentRow].GetType().GetField(dataEntry.Name).SetValue(consumablesDatabase.entries[currentRow], boolVal);
            //Debug.Log("Current item being added to database is " + consumablesDatabase.entries[currentRow].itemName + ". It's value is " + boolVal);
        }
        else if (columnDataTypes[currentColumnEntry] == "float")
        {
            float floatVal = float.Parse(data[numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry]);
            consumablesDatabase.entries[currentRow].GetType().GetField(dataEntry.Name).SetValue(consumablesDatabase.entries[currentRow], floatVal);
            //Debug.Log("Current item being added to database is " + consumablesDatabase.entries[currentRow].itemName + ". It's value is " + floatVal);
        }
        else { /*Debug.Log("No data type match");*/ }

        
    }

    // SUPPORT FUNCTIONS
    private List<string> GetColumnData(string[] data, string typeOfList)
    {
        List<string> toReturn = new List<string>();

        consumablesDatabase.entries[0] = new Consumables();

        FieldInfo[] fieldsToLoop = consumablesDatabase.entries[0].GetType().GetFields();

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

    public string ReturnNameFromID(int id) 
    {
        string toReturn = "No Match"; Debug.Log("ReturnNameFromID in ConsumablesDatabase.cs returned no match");
        for(int i = 0; i < consumablesDatabase.entries.Length; i++)
        {
            if (id == consumablesDatabase.entries[i].id)
            {
                toReturn = consumablesDatabase.entries[i].itemName;
            }
        }
        return toReturn;
    }

    public int ReturnIDfromName(string name)
    {
        int toReturn = -1; Debug.Log("ReturnNameFromID in ConsumablesDatabase.cs returned no match");
        for (int i = 0; i < consumablesDatabase.entries.Length; i++)
        {
            if (name == consumablesDatabase.entries[i].itemName)
            {
                toReturn = consumablesDatabase.entries[i].id;
            }
        }
        return toReturn;
    }

}
