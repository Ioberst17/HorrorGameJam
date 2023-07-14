using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

public class QuestUpdaterSupport : MonoBehaviour
{
    // external reference
    QuestManager questManager;

    private void Start() { questManager = FindObjectOfType<QuestManager>(); }

    [System.Serializable]
    public class QuestInformationToSend
    {
        public string questName; // The name of the quest
        public int subQuestIndex; // The index of the sub-quest
    }

    public List<QuestInformationToSend> questFunctions; // List of quest information to listen for

    public void UpdateQuest()
    {
        foreach (QuestInformationToSend questInfo in questFunctions) { questManager.UpdateQuestStatus(questInfo.questName, questInfo.subQuestIndex); }
    }
}
