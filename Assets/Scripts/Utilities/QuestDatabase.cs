using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDatabase : Database<Quest>
{
    private void Awake()
    {
        // load quest database
        numOfColumns = 8; // must be updated as CSV is updated
        textAssetData = Resources.Load<TextAsset>("TextFiles/Quests/QuestDatabase");
        string[] data = ReadCSV();
        CreateDatabase(data);
    }
}
