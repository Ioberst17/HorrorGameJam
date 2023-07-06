using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class QuestManager : MonoBehaviour
{
    private DataManager dataManager;
    public List<Quest> quests;
    private QuestDatabase questDatabase;
    private SubQuestDatabase subQuestDatabase;
    private QuestUpdater questUpdater;
    [SerializeField] private Quest questChecker;
    [SerializeField] private Quest activeQuest;
    public int activeQuestIndex;

    private void Start()
    {
        questDatabase = FindObjectOfType<QuestDatabase>();
        subQuestDatabase = FindObjectOfType<SubQuestDatabase>();
        questUpdater = GetComponent<QuestUpdater>();
        dataManager = DataManager.Instance;
        Load();
        StartQuest("Tutorial");
    }

    // load active quests and states from DataManager
    private void Load()
    {
        if (dataManager.sessionData.quests == null) { quests = new List<Quest>(); }
        else { quests = dataManager.sessionData.quests; }
    }
    // save active quests and state to DataManager
    private void Save() { dataManager.sessionData.quests = quests; }

    public void StartQuest(string questName) 
    {
        Quest questToUpdate = questDatabase.data.entries.FirstOrDefault(q => q.name == questName); // find a quest with said name
        if (questToUpdate != null) 
        {
            quests.Add(questToUpdate);
            questToUpdate.isStarted = true;
            SetQuestActiveStatus(true, questName);
        } 
        else { Debug.Log("Quest name could not be found; could not change active status!"); }
    }

    // make a quest the active or non-active quest
    public void SetQuestActiveStatus(bool state, string questName) 
    {
        // find a quest with said name
        Quest questToUpdate = quests.FirstOrDefault(q => q.name == questName); 

        // if said name is found, change it's state
        if (questToUpdate != null) 
        { 
            questToUpdate.isActive = state; 
            activeQuest = questToUpdate; 
        } 
        else { Debug.Log("Quest name could not be found; could not change active status!"); }

        // make sure it is the only active quest
        if (state == true) { CheckIfMultipleQuestsActive(activeQuest.name); }
    }

    // turns off active status of any quest that doesn't have the questname
    void CheckIfMultipleQuestsActive(string questName)
    {
        foreach(Quest quest in quests) { if (quest.name != questName && quest.isActive) { quest.isActive = false; } }
    }

    public void UpdateQuestStatus(string questName, int index)
    {
        Debug.Log("Quest status update attempt");
        if (!QuestNameIsValid(questName)) { Debug.Log("Not a valid quest name! Check the source is using a proper quest name that is stored in Quest Manager and Quest Database"); return; }
        UpdateQuestHelper(index);
    }

    public void UpdateQuestStatus(int questID, int index)
    {
        if (!QuestIDIsValid(questID)) { Debug.Log("Not a valid quest ID! Check the source is using a proper quest ID that is stored in Quest Manager and Quest Database"); return; }
        UpdateQuestHelper(index);
    }

    void UpdateQuestHelper(int index)
    {
        // if the quest isn't started or completed, return
        if (!questChecker.isStarted || questChecker.isCompleted) { return; }

        if (questChecker.currentSubQuestIndex == index) 
        { 
            questChecker.currentSubQuestIndex++; 

            Debug.Log("Task " + questChecker.currentSubQuestIndex + " is complete!");
            if (QuestCompletionCheck(questChecker))
            {
                // if quest is complete don't need to do anything additional
            } 
            else
            {
                // in the subquest database, if for the current quest, and current activity needs dedicated tracking
                if (subQuestDatabase.dataArray[questChecker.id].entries[questChecker.currentSubQuestIndex].needsDedicatedTracking)
                {
                    questUpdater.TurnOnTracking(questChecker.name, questChecker.currentSubQuestIndex);
                }
            }
        }
        else { } // do nothing, since the activity isn't in the right order
    }

    bool QuestIDIsValid(int questID)  { return (questChecker = quests.FirstOrDefault(q => q.id == questID)) != null; }

    bool QuestNameIsValid(string questName) { return (questChecker = quests.FirstOrDefault(q => q.name == questName)) != null; }

    bool QuestCompletionCheck(Quest quest) { if (quest.currentSubQuestIndex == quest.numOfSubQuests - 1) { quest.isCompleted = true; return true; } return false; }
}
