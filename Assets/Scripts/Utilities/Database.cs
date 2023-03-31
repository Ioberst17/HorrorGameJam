using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Database<T> : MonoBehaviour
{
    public TextAsset textAssetData; // the CSV to read from, must be assigned in Inspector
    public int numOfColumns;

    [SerializeField]
    private List<string> columnNames;
    [SerializeField]
    private List<string> columnDataTypes;

    [Serializable]
    public class DB // create the a database of all game items
    {
        public T[] entries;
    }

    public DB data = new DB();

    public string[] ReadCSV()
    {
        string[] data = textAssetData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        return data;
    }

    public virtual void CreateDatabase(string[] data)
    {
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
            int intVal = int.Parse(data[numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry]);
            this.data.entries[currentRow].GetType().GetField(dataEntry.Name).SetValue(this.data.entries[currentRow], intVal);
        }
        else if (columnDataTypes[currentColumnEntry] == "string")
        {
            string strVal = data[numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry];
            this.data.entries[currentRow].GetType().GetField(dataEntry.Name).SetValue(this.data.entries[currentRow], strVal);
        }
        else if (columnDataTypes[currentColumnEntry] == "bool")
        {
            bool boolVal = bool.Parse(data[numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry]);
            this.data.entries[currentRow].GetType().GetField(dataEntry.Name).SetValue(this.data.entries[currentRow], boolVal);
        }
        else if (columnDataTypes[currentColumnEntry] == "float")
        {
            float floatVal = float.Parse(data[numOfColumns * (currentRow + rowsToSkip) + currentColumnEntry]);
            this.data.entries[currentRow].GetType().GetField(dataEntry.Name).SetValue(this.data.entries[currentRow], floatVal);
        }
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
}
