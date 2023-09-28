using Ink.Parsed;
using Ink.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using static SubQuest;

public class Database<T> : MonoBehaviour where T : IDatabaseItem
{
    [Header("Generic Information On the Database")]
    public int numOfColumns;
    public int numOfDataRows;
    private bool foundItemToReturn;

    // for CSV Header Rows
    [SerializeField] private List<string> columnNames = new();
    [SerializeField] private List<string> columnDataTypes = new();
    private List<string> columnItemClass = new();
    private List<string> columnClassFieldOrProperty = new();
    private List<string> columnDataTypesLong = new();

    // Internal variables for building
    private string[] lines, lineData, rawData;
    private int IndexForClassProperties, IndexForClassField;
    private string columnName, dataType;
    private int rowsToSkip = 5; // this refers to the 'header rows' in the CSVs that describe data
    public int length;
    PropertyInfo[] propertyInfo;
    FieldInfo[] fieldInfo;
    public string[] dataView;

    [Header("Single Database Variables")]
    public TextAsset textAssetData; // the CSV to read from, must be assigned in Inspector
    [Serializable]
    public class DB  { public T[] entries; }
    public DB data = new();

    [Header("Array of Databases Variables")]
    [SerializeField] public TextAsset[] arrayOfTextAssetData;
    [SerializeField] public DB[] dataArray;

    public string[] ReadCSV(TextAsset specificDataToUse = null)
    {
        // get the inital set of lines from a either textAssetData or an input source
        if (specificDataToUse == null) { lines = textAssetData.text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries); }
        else {  lines = specificDataToUse.text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries); }

        // Create the data variables based on the num of Columns and the length of lines (set above)
        rawData = new string[lines.Length * numOfColumns]; 
        int dataIndex = 0;

        // loop through each line and split each line into it's individual cells and into 'data' variables
        foreach (string line in lines)
        {
            lineData = line.Split(',');
            Array.Copy(lineData, 0, rawData, dataIndex * numOfColumns, numOfColumns); 
            dataIndex++;
        }

        dataView = rawData;
        return rawData;
    }

    // after readcsv is called, a database can be created using the now usable data variable
    public virtual void CreateDatabase(string[] data)
    {
        UpdateColumnData(data); // get column names, which will be used in database creation
        UpdateRowNum(data); // get row numbers to know number of entries to make

        // set up data to be edited, give it the right number of rows, and the number of entries should = the default number of the type
        length = data.Length;
        this.data.entries = new T[numOfDataRows];
        this.data.entries[0] = (T)Activator.CreateInstance(typeof(T));

        // update the class reference to understand number of fields and properties when parsing
        T currentInstance = (T)Activator.CreateInstance(typeof(T));
        UpdateClassInfoReference(currentInstance);

        // loop through each row
        for (int i = 0; i < numOfDataRows; i++)
        {
            this.data.entries[i] = (T)Activator.CreateInstance(typeof(T)); // set this entry to an 'instance' of the type
            // get to each column
            for (int j = 0; j < columnNames.Count; j++)
            {
                columnName = columnNames[j];
                dataType = columnDataTypes[j];

                // assume it is neither a field or property (has an index of -1)
                IndexForClassProperties = -1; IndexForClassField = -1;
                // now check for the index, if it exists, it will not be -1
                IndexForClassField = Array.FindIndex(fieldInfo, p => p.Name == columnName);
                IndexForClassProperties = Array.FindIndex(propertyInfo, p => p.Name == columnName);

                // parse data in as needed
                if (IndexForClassField >= 0) { ParseDataToTable(i, j, data, fieldInfo[IndexForClassField], dataType, this.data.entries[i]); }
                if (IndexForClassProperties >= 0) { ParseDataToTable(i, j, data, propertyInfo[IndexForClassProperties], dataType, this.data.entries[i]); }

                // error handling
                if (IndexForClassProperties !>=0 && IndexForClassField! >= 0) { Debug.Log("Current row is: " + i + "; Current column is: " + j + "; did not find a prop or field in class that matches"); }
            }
        }
    }

    // creates a series of databases from an array of text asset data
    // assumes all text asset data should be of the same type
    // requires arrayOFTextAssetData is loaded in prior to calling this function
    public virtual void CreateDatabases()
    {
        // initialize the array of data
        this.dataArray = new DB[arrayOfTextAssetData.Length];
        string[] initialDataForColumn = ReadCSV(arrayOfTextAssetData[0]);
        UpdateColumnData(initialDataForColumn); // get column names, which will be used in database creation, need to get this only once before looping through

        // create a database for each text asset
        for (int x = 0; x < arrayOfTextAssetData.Length; x++)
        {
            // read in data
            string[] data = ReadCSV(arrayOfTextAssetData[x]);
            UpdateRowNum(data); // get row numbers to know number of entries to make

            // set up data to be edited, give it the right number of rows, and the number of entries should = the default number of the type
            length = data.Length;
            dataArray[x] = new();
            dataArray[x].entries = new T[numOfDataRows];
            dataArray[x].entries[0] = (T)Activator.CreateInstance(typeof(T));

            // update the class reference to understand number of fields and properties when parsing
            T currentInstance = (T)Activator.CreateInstance(typeof(T));
            UpdateClassInfoReference(currentInstance);

            // loop through each row
            for (int i = 0; i < numOfDataRows; i++)
            {
                //this.dataArray[x].entries = new T[numOfDataRows];
                this.dataArray[x].entries[i] = (T)Activator.CreateInstance(typeof(T)); // set this entry to an 'instance' of the type
                // get to each column
                for (int j = 0; j < columnNames.Count; j++)
                {
                    columnName = columnNames[j];
                    dataType = columnDataTypes[j];

                    // assume it is neither a field or property (has an index of -1)
                    IndexForClassProperties = -1; IndexForClassField = -1;
                    // now check for the index, if it exists, it will not be -1
                    IndexForClassField = Array.FindIndex(fieldInfo, p => p.Name == columnName);
                    IndexForClassProperties = Array.FindIndex(propertyInfo, p => p.Name == columnName);

                    // parse data in as needed
                    if (IndexForClassField >= 0) { ParseDataToTable(i, j, data, fieldInfo[IndexForClassField], dataType, this.dataArray[x].entries[i]); }
                    if (IndexForClassProperties >= 0) { ParseDataToTable(i, j, data, propertyInfo[IndexForClassProperties], dataType, this.dataArray[x].entries[i]); }

                    // error handling
                    if (IndexForClassProperties! >= 0 && IndexForClassField! >= 0) { Debug.Log("Current row is: " + i + "; Current column is: " + j + "; did not find a prop or field in class that matches"); }
                }
            }
        }
    }

    // big function that parses in different string data as the needed data type for a field or a property e.g.
    // a value of 12 can be parsed as an int, or a TRUE can be parsed in as a bool for either a field or a property
    public void ParseDataToTable<F>(int currentRow, int currentColumnEntry, string[] data, F dataEntry, string dataType, T entryToChange) where F : MemberInfo
    {
        if (currentColumnEntry < 0 || currentColumnEntry >= columnDataTypes.Count)
        {
            Debug.LogWarning("Invalid column index: " + currentColumnEntry);
            return;
        }

        string columnDataType = columnDataTypes[currentColumnEntry];

        try
        {
            // Define a dictionary to map column data types to parsing and setting operations
            Dictionary<string, Action<string>> typeActions = new()
            {
                { "int", (value) => { int intVal = int.Parse(value); SetValue(entryToChange, dataEntry, intVal); } },
                { "string", (value) => { SetValue(entryToChange, dataEntry, value); } },
                { "bool", (value) => { bool boolVal = bool.Parse(value); SetValue(entryToChange, dataEntry, boolVal); } },
                { "float", (value) => { float floatVal = float.Parse(value); SetValue(entryToChange, dataEntry, floatVal); } },
                { "sprite", (value) => { Sprite spriteVal = Resources.Load<Sprite>(value); SetValue(entryToChange, dataEntry, spriteVal); } },
                { "SubQuestType", (value) => { SubQuestType enumVal = (SubQuestType)Enum.Parse(typeof(SubQuestType), value); SetValue(entryToChange, dataEntry, enumVal); } },
                { "SubQuest", (value) =>  { SetValue<MemberInfo, List<SubQuest>>(entryToChange, dataEntry, null); } }
            };

            // if the dictionary contains the value that is of the type of the columnDataType -> try and parse it in
            if (typeActions.ContainsKey(columnDataType))
            {
                string value = data[numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry];
                typeActions[columnDataType].Invoke(value);
            }
            else { Debug.Log("No data type match. Current column is " + columnNames[currentColumnEntry] + " with data type " + columnDataType); }
        }
        catch (Exception e)
        {
            ErrorHandler(e, currentColumnEntry, currentRow, rowsToSkip, data, dataEntry);
        }
    }


    // reads the value into the entry to change depending on whether it is meant to be a field or property value
    private void SetValue<F, V>(T entryToChange, F dataEntry, V value) where F : MemberInfo
    {
        if (dataEntry is FieldInfo fieldInfo) { fieldInfo.SetValue(entryToChange, value); }
        else if (dataEntry is PropertyInfo propertyInfo) { propertyInfo.SetValue(entryToChange, value); }
    }

    void UpdateClassInfoReference<I>(I item)
    {
        propertyInfo = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.PropertyType != typeof(SubQuestType)).ToArray();
        fieldInfo = item.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).Where(f => f.FieldType != typeof(SubQuestType)).ToArray();

    }

    public void UpdateColumnData(string[] data)
    {
        for (int i = 0; i < numOfColumns; i++)
        {
            columnNames.Add(data[i]);
            columnDataTypes.Add(data[(numOfColumns) + i]);
            columnItemClass.Add(data[(numOfColumns * 2) + i]);
            columnClassFieldOrProperty.Add(data[(numOfColumns * 3) + i]);
            columnDataTypesLong.Add(data[(numOfColumns * 4) + i]);
        }
    }


    private void UpdateRowNum(string[] data) { numOfDataRows = data.Length / numOfColumns - rowsToSkip; }


    private void ErrorHandler(Exception e, int currentColumnEntry, int currentRow, int rowsToSkip, string[] data, MemberInfo dataEntry)
    {
        int actualRow = currentRow + rowsToSkip;
        Debug.Log("Caught an error: " + e.Message);
        Debug.Log(
            "The name of the current column being read is: " + columnNames[currentColumnEntry] +
            "\nThe type of the current column being read is: " + columnDataTypes[currentColumnEntry] +
            "\nThe current column being evaluated is: " + currentColumnEntry +
            "\nThe current row being evaluated is: " + actualRow +
            "\nThe index of the data array is: " + (numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry) +
            "\nThe value being parsed is: " + data[numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry]);
        if (dataEntry != null)
        {
            if(dataEntry is FieldInfo) Debug.Log("The entry being added to the database object is named: " + this.data.entries[currentRow].GetType().GetField(dataEntry?.Name));
            if(dataEntry is PropertyInfo) Debug.Log("The entry being added to the database object is named: " + this.data.entries[currentRow].GetType().GetProperty(dataEntry?.Name));
        }

            
    }

    // Generic Functions for Child Databases

    public T ReturnItemFromID(int id)
    {
        foundItemToReturn = false;
        for (int i = 0; i < data.entries.Length; i++) { if (id == data.entries[i].id) { foundItemToReturn = true; return data.entries[i]; } }
        if(foundItemToReturn == false) { Debug.LogFormat("Could not find item with ID: {0}; Check for presence or mispellings", id); }
        return default;
    }

    public T ReturnItemFromName(string name)
    {
        foundItemToReturn = false;
        for (int i = 0; i < data.entries.Length; i++) { if (name == data.entries[i].name) { foundItemToReturn = true; return data.entries[i]; } }
        if (foundItemToReturn == false) { Debug.LogFormat("Could not find item with name: {0}; Check for presence or mispellings", name); }
        return default;
    }

}
