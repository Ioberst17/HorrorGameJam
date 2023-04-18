using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class Database<T> : MonoBehaviour where T : IShoppable
{
    public TextAsset textAssetData; // the CSV to read from, must be assigned in Inspector
    public int numOfColumns;
    [SerializeField] bool hasCostField;
    public int numOfDataRows;

    // for CSV Header Rows
    [SerializeField] private List<string> columnNames = new List<string>();
    [SerializeField] private List<string> columnDataTypes = new List<string>();
    [SerializeField] private List<string> columnItemClass = new List<string>();
    [SerializeField] private List<string> columnClassFieldOrProperty = new List<string>();
    [SerializeField] private List<string> columnDataTypesLong = new List<string>();

    // Internal variables for building
    private int searchIndexerProp, searchIndexerField;
    private string columnName, dataType;
    private bool matchFound;
    private int rowsToSkip = 5; // this refers to the 'header rows' in the CSVs that describe data
    public int length;
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
        hasCostField = typeof(T).GetProperties().Any(prop => prop.Name == "cost");

        UpdateColumnData(data);
        UpdateRowNum(data);

        length = data.Length;
        this.data.entries = new T[numOfDataRows];
        this.data.entries[0] = (T)Activator.CreateInstance(typeof(T));

        T currentInstance = (T)Activator.CreateInstance(typeof(T));
        PropertyInfo[] propertyInfo = currentInstance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        FieldInfo[] fieldInfo = currentInstance.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

        //if (propertyInfo.Length == 0)
        //{
        //    Debug.LogWarning("No public properties found in type: " + typeof(T));
        //    return;
        //}

        //if (fieldInfo.Length == 0)
        //{
        //    Debug.LogWarning("No public fields found in type: " + typeof(T));
        //    return;
        //}

        if (currentInstance is Consumables)
        {
            var item = currentInstance as Consumables;
            propertyInfo = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            fieldInfo = item.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        }
        else if (currentInstance is Weapons)
        {
            var item = currentInstance as Weapons;
            propertyInfo = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            fieldInfo = item.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        }
        else if (currentInstance is NarrativeItems)
        {
            var item = currentInstance as NarrativeItems;
            propertyInfo = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            fieldInfo = item.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        }

        Debug.Log("Working on type of: " + typeof(T));
        Debug.Log("PropertyInfo length is: " + propertyInfo.Length);
        Debug.Log("FieldInfo length is: " + fieldInfo.Length);

        for (int i = 0; i < numOfDataRows; i++)
        {
            this.data.entries[i] = (T)Activator.CreateInstance(typeof(T));
            for (int j = 0; j < columnNames.Count; j++)
            {
                columnName = columnNames[j];
                dataType = columnDataTypes[j];
                Debug.Log("Current row is: " + i + "; Current column is: " + j + "; Column name is: " + columnName);

                searchIndexerProp = -1; searchIndexerField = -1;

                searchIndexerField = Array.FindIndex(fieldInfo, p => p.Name == columnName);
                var fieldInfoObj = Array.Find(fieldInfo, p => p.Name == columnName);
                if (fieldInfoObj != null) { Debug.Log("Looking for " + fieldInfoObj.Name + "; its data type is: " + fieldInfoObj.FieldType + ". Checking against name: " + columnName); }
                if (searchIndexerField >= 0)
                {
                    Debug.Log("Current row is: " + i + "; Current column is: " + j + "; Found a field; It's name is: " + columnName);
                    if (j > fieldInfo.Length) { Debug.Log("Column index is: " + j + " is greater than fieldInfo.Length: " + fieldInfo.Length); }
                    ParseDataToTable(i, j, data, fieldInfo[searchIndexerField], dataType);
                }
                searchIndexerProp = Array.FindIndex(propertyInfo, p => p.Name == columnName);
                var propInfoObj = Array.Find(propertyInfo, p => p.Name == columnName);
                if (propInfoObj != null) { Debug.Log("Looking for " + propInfoObj.Name + "; its data type is: " + propInfoObj.PropertyType + ". Checking against name: " + columnName); }
                if (searchIndexerProp >= 0)
                {
                    if (columnName == "cost" && hasCostField && this.data.entries[i] is IShoppable itemWithCostProperty)
                    {
                        Debug.Log("Current row is: " + i + "; Current column is: " + j + "; Found a prop; It's name is: " + columnName);
                        //var (columnIndex, _) = GetColumnInfo(data, columnName);
                        //int cost = int.Parse(data[numOfColumns * (i + rowsToSkip) + j]);
                        //itemWithCostProperty.cost = cost;
                        ParseDataToTable(i, j, data, propertyInfo[searchIndexerProp], dataType);
                    }
                }
                if (searchIndexerProp !>=0 && searchIndexerField! >= 0)
                {
                    Debug.Log("Current row is: " + i + "; Current column is: " + j + "; did not find a prop or field");
                }
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
        else if (columnDataTypes[currentColumnEntry] == "sprite") { }
        else { Debug.Log("No data type match. Current column is " + columnNames[currentColumnEntry] + " with data type " + columnDataTypes[currentColumnEntry]); }
    }

    private (int, string) GetColumnInfo(string[] data, string columnName)
    {
        int columnIndex = -1;
        string dataType = "";

        for (int i = 0; i < numOfColumns; i++)
        {
            if (data[i] == columnName)
            {
                columnIndex = i;
                dataType = data[numOfColumns + i];
                break;
            }
        }

        return (columnIndex, dataType);
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
    
}
