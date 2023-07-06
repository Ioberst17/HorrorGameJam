using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubQuestDatabase : Database<SubQuest>
{
    QuestDatabase questDatabase;

    void Awake()
    {
        // load quest data with subquests
        numOfColumns = 5;
        arrayOfTextAssetData = Resources.LoadAll<TextAsset>("TextFiles/Quests/SubQuests");
        CreateDatabases();
    }

    private void Start()
    {
        // call after Quest DB is initialized
        questDatabase = GetComponent<QuestDatabase>();
        UpdateQuestData();
    }

    private void UpdateQuestData()
    {
        for (int i = 0; i < questDatabase.data.entries.Length; i++) 
        {
            // set the number of sub quests = to the number of activities in the associated subquest collection
            questDatabase.data.entries[i].numOfSubQuests = this.dataArray[i].entries.Length;
        }
    }
}
