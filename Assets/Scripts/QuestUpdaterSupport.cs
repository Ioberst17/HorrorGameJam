using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class QuestUpdaterSupport : MonoBehaviour
{
    [System.Serializable]
    public class QuestInformationToSend
    {
        public string questName; // The name of the quest
        public int subQuestIndex; // The index of the sub-quest
    }

    public List<QuestInformationToSend> questFunctions; // List of quest information to listen for
    private QuestManager questManager; // Reference to the QuestManager

    private void Start()
    {
        questManager = FindObjectOfType<QuestManager>(); // Find the QuestManager in the scene

        if (questManager == null)
        {
            Debug.LogError("QuestManager not found in the scene.");
            return;
        }
    }

    public void UpdateQuest()
    {
        foreach (QuestInformationToSend functionInfo in questFunctions)
        {
            questManager.UpdateQuestStatus(functionInfo.questName, functionInfo.subQuestIndex);
        }
    }

}


