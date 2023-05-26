using Ink.Parsed;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class Database<T> : MonoBehaviour where T : IDatabaseItem, IShoppable
{
    public TextAsset textAssetData; // the CSV to read from, must be assigned in Inspector
    public int numOfColumns;
    public int numOfDataRows;
    private T returnedItem;
    private bool foundItemToReturn;

    // for CSV Header Rows
    [SerializeField] private List<string> columnNames = new List<string>();
    [SerializeField] private List<string> columnDataTypes = new List<string>();
    [SerializeField] private List<string> columnItemClass = new List<string>();
    [SerializeField] private List<string> columnClassFieldOrProperty = new List<string>();
    [SerializeField] private List<string> columnDataTypesLong = new List<string>();

    // Internal variables for building
    private int IndexForClassProperties, IndexForClassField;
    private string columnName, dataType;
    private int rowsToSkip = 5; // this refers to the 'header rows' in the CSVs that describe data
    public int length;
    PropertyInfo[] propertyInfo;
    FieldInfo[] fieldInfo;
    [SerializeField] public string[] dataView;

    [Serializable]
    public class DB // create the a database of all game items
    {
        public T[] entries;
    }

    public DB data = new DB();

    public string[] ReadCSV()
    {
        string[] lines = textAssetData.text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        string[] data = new string[lines.Length * numOfColumns];
        int dataIndex = 0;

        foreach (string line in lines)
        {
            string[] lineData = line.Split(',');
            Array.Copy(lineData, 0, data, dataIndex * numOfColumns, numOfColumns);
            dataIndex++;
        }

        dataView = data;
        return data;
    }


    public virtual void CreateDatabase(string[] data)
    {
        UpdateColumnData(data); // get column names, which will be used in database creation
        UpdateRowNum(data); // get row numbers to know number of entries to make

        length = data.Length;
        this.data.entries = new T[numOfDataRows];
        this.data.entries[0] = (T)Activator.CreateInstance(typeof(T));

        T currentInstance = (T)Activator.CreateInstance(typeof(T));
        UpdateClassInfoReference(currentInstance);

        for (int i = 0; i < numOfDataRows; i++)
        {
            this.data.entries[i] = (T)Activator.CreateInstance(typeof(T));
            for (int j = 0; j < columnNames.Count; j++)
            {
                columnName = columnNames[j];
                dataType = columnDataTypes[j];

                IndexForClassProperties = -1; IndexForClassField = -1;

                IndexForClassField = Array.FindIndex(fieldInfo, p => p.Name == columnName);
                var fieldInfoObj = Array.Find(fieldInfo, p => p.Name == columnName);
                if (IndexForClassField >= 0) { ParseDataToTable(i, j, data, fieldInfo[IndexForClassField], dataType); }

                IndexForClassProperties = Array.FindIndex(propertyInfo, p => p.Name == columnName);
                var propInfoObj = Array.Find(propertyInfo, p => p.Name == columnName);
                if (IndexForClassProperties >= 0) { ParseDataToTable(i, j, data, propertyInfo[IndexForClassProperties], dataType); }

                if (IndexForClassProperties !>=0 && IndexForClassField! >= 0) { Debug.Log("Current row is: " + i + "; Current column is: " + j + "; did not find a prop or field in class that matches"); }
            }
        }
    }

    public void ParseDataToTable<F>(int currentRow, int currentColumnEntry, string[] data, F dataEntry, string dataType) where F : MemberInfo
    {
        if (currentColumnEntry < 0 || currentColumnEntry >= columnDataTypes.Count)
        {
            Debug.LogWarning("Invalid column index: " + currentColumnEntry);
            return;
        }

        if (columnDataTypes[currentColumnEntry] == "int")
        {
            try
            {
                int intVal = int.Parse(data[numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry]);
                if (dataEntry is FieldInfo fieldInfo) { fieldInfo.SetValue(this.data.entries[currentRow], intVal);}
                else if (dataEntry is PropertyInfo propertyInfo) { propertyInfo.SetValue(this.data.entries[currentRow], intVal); }
            }
            catch (Exception e) { ErrorHandler(e, currentColumnEntry, currentRow, rowsToSkip, data, dataEntry); }
        }
        else if (columnDataTypes[currentColumnEntry] == "string")
        {
            try
            {
                string strVal = data[numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry];
                if (dataEntry is FieldInfo fieldInfo) { fieldInfo.SetValue(this.data.entries[currentRow], strVal); }
                else if (dataEntry is PropertyInfo propertyInfo) { propertyInfo.SetValue(this.data.entries[currentRow], strVal); }
            }
            catch (Exception e) { ErrorHandler(e, currentColumnEntry, currentRow, rowsToSkip, data, dataEntry); }
        }
        else if (columnDataTypes[currentColumnEntry] == "bool")
        {
            try
            {
                bool boolVal = bool.Parse(data[numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry]);
                if (dataEntry is FieldInfo fieldInfo) { fieldInfo.SetValue(this.data.entries[currentRow], boolVal); }
                else if (dataEntry is PropertyInfo propertyInfo) { propertyInfo.SetValue(this.data.entries[currentRow], boolVal);}
            }
            catch (Exception e) { ErrorHandler(e, currentColumnEntry, currentRow, rowsToSkip, data, dataEntry); }
        }
        else if (columnDataTypes[currentColumnEntry] == "float")
        {
            try
            {
                float floatVal = float.Parse(data[numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry]);
                if (dataEntry is FieldInfo fieldInfo) { fieldInfo.SetValue(this.data.entries[currentRow], floatVal); }
                else if (dataEntry is PropertyInfo propertyInfo) { propertyInfo.SetValue(this.data.entries[currentRow], floatVal); }
            }
            catch (Exception e) { ErrorHandler(e, currentColumnEntry, currentRow, rowsToSkip, data, dataEntry); }
        }
        else if (columnDataTypes[currentColumnEntry] == "sprite") 
        {
            try
            {
                string strVal = data[numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry];
                Sprite spriteVal = Resources.Load<Sprite>(strVal);
                if (dataEntry is FieldInfo fieldInfo) { fieldInfo.SetValue(this.data.entries[currentRow], spriteVal); }
                else if (dataEntry is PropertyInfo propertyInfo) { propertyInfo.SetValue(this.data.entries[currentRow], spriteVal); }
            }
            catch (Exception e) { ErrorHandler(e, currentColumnEntry, currentRow, rowsToSkip, data, dataEntry); }
        }
        else { Debug.Log("No data type match. Current column is " + columnNames[currentColumnEntry] + " with data type " + columnDataTypes[currentColumnEntry]); }
    }

    void UpdateClassInfoReference<I>(I item)
    {
        propertyInfo = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        fieldInfo = item.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
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
        for (int i = 0; i < this.data.entries.Length; i++) { if (id == this.data.entries[i].id) { foundItemToReturn = true; return this.data.entries[i]; } }
        if(foundItemToReturn == false) { Debug.LogFormat("Could not find item with ID: {0}; Check for presence or mispellings", id); }
        return default(T);
    }

    public T ReturnItemFromName(string name)
    {
        foundItemToReturn = false;
        for (int i = 0; i < this.data.entries.Length; i++) { if (name == this.data.entries[i].name) { foundItemToReturn = true; return this.data.entries[i]; } }
        if (foundItemToReturn == false) { Debug.LogFormat("Could not find item with name: {0}; Check for presence or mispellings", name); }
        return default(T);
    }

}
