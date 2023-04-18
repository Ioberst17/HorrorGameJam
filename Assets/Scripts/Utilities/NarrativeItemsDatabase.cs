using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeItemsDatabase : Database<NarrativeItems>
{
    // Start is called before the first frame update
    private void Awake()
    {
        numOfColumns = 13; // must be updated as CSV is updated
        textAssetData = Resources.Load<TextAsset>("TextFiles/NarrativeItemsDatabase");
        string[] data = ReadCSV();
        CreateDatabase(data);
    }
}
