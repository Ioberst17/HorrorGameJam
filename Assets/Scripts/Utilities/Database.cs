using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class Database<T> : MonoBehaviour
{
    public TextAsset textAssetData; // the CSV to read from, must be assigned in Inspector
    public int numOfColumns;

    [SerializeField]
    private List<string> columnNames;
    [SerializeField]
    private List<string> columnDataTypes;

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
        length = data.Length;
        int numOfRows = data.Length / numOfColumns - 2; // gets data length (total # of cells), then divides by # of columns to get # of rowsl -2 for header and datatype row
        this.data.entries = new T[numOfRows];

        columnNames = GetColumnData(data, "columnNames");
        columnDataTypes = GetColumnData(data, "variableDataTypes");

        for (int i = 0; i < numOfRows; i++)
        {
            this.data.entries[i] = (T)Activator.CreateInstance(typeof(T));

            FieldInfo[] columnsToFill = this.data.entries[i].GetType().GetFields();

            for (int j = 0; j < columnsToFill.Length; j++)
            {
                foreach (var dataEntry in columnsToFill)
                {
                    if (dataEntry.Name == columnNames[j]) { ParseDataToTable(i, j, data, dataEntry); }
                }
            }
        }
    }

    public void ParseDataToTable(int currentRow, int currentColumnEntry, string[] data, FieldInfo dataEntry)
    {
        int rowsToSkip = 2; //header row and datatype row are skipped when parsing

        if (columnDataTypes[currentColumnEntry] == "int")
        {
            try
            {
                int intVal = int.Parse(data[numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry]);
                this.data.entries[currentRow].GetType().GetField(dataEntry.Name).SetValue(this.data.entries[currentRow], intVal);
            }
            catch (Exception e) { ErrorHandler(e, currentColumnEntry, currentRow, rowsToSkip, data, dataEntry); }
        }
        else if (columnDataTypes[currentColumnEntry] == "string")
        {
            try
            {
                string strVal = data[numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry];
                this.data.entries[currentRow].GetType().GetField(dataEntry.Name).SetValue(this.data.entries[currentRow], strVal);
            }
            catch (Exception e) { ErrorHandler(e, currentColumnEntry, currentRow, rowsToSkip, data, dataEntry); }
        }
        else if (columnDataTypes[currentColumnEntry] == "bool")
        {
            try
            {
                bool boolVal = bool.Parse(data[numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry]);
                this.data.entries[currentRow].GetType().GetField(dataEntry.Name).SetValue(this.data.entries[currentRow], boolVal);
            }
            catch (Exception e) { ErrorHandler(e, currentColumnEntry, currentRow, rowsToSkip, data, dataEntry); }
        }
        else if (columnDataTypes[currentColumnEntry] == "float")
        {
            try
            {
                float floatVal = float.Parse(data[numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry]);
                this.data.entries[currentRow].GetType().GetField(dataEntry.Name).SetValue(this.data.entries[currentRow], floatVal);
            }
            catch (Exception e) { ErrorHandler(e, currentColumnEntry, currentRow, rowsToSkip, data, dataEntry); }

        }
        else if (columnDataTypes[currentColumnEntry] == "sprite") { }
        else { Debug.Log("No data type match. Current column is " + columnNames[currentColumnEntry] + " with data type " + columnDataTypes[currentColumnEntry]); }
    }

    private List<string> GetColumnData(string[] data, string typeOfList)
    {
        List<string> toReturn = new List<string>();

        this.data.entries[0] = (T)Activator.CreateInstance(typeof(T));

        FieldInfo[] fieldsToLoop = this.data.entries[0].GetType().GetFields();

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

    private void ErrorHandler(Exception e, int currentColumnEntry, int currentRow, int rowsToSkip, string[] data, FieldInfo dataEntry)
    {
        int actualRow = currentRow + rowsToSkip;
        Debug.Log("Caught an error: " + e.Message);
        Debug.Log("The name of the current column being read is: " + columnNames[currentColumnEntry] +
            "\nThe type of the current column being read is: " + columnDataTypes[currentColumnEntry] +
            "\nThe current column being evaluated is: " + currentColumnEntry +
            "\nThe current row being evaluated is: " + actualRow +
            "\nThe index of the data array is: " + numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry + 
            "\nThe value being parsed is: " + data[numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry] +
            "\nThe entry being added to the database object is named: " + this.data.entries[currentRow].GetType().GetField(dataEntry.Name));
    }
    
}
