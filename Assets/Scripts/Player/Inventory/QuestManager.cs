using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class QuestManager : MonoBehaviour
{
    // external references e.g. data storage and all quest data
    private DataManager dataManager;
    private QuestDatabase questDatabase;
    private SubQuestDatabase subQuestDatabase;

    // external references that will update questManager when a subquest activity is completed
    private QuestUpdater questUpdater; // has memory so it can track an activity with multiple activities e.g. 'talk to all NPCs'
    private QuestUpdaterSupport questUpdaterSupport; // used for binary (completed / not completed events) like when a player enters a new area

    // internal quest variables to track
    public List<Quest> quests;
    private Quest questChecker;
    public Quest activeQuest;

    private void Start()
    {
        questDatabase = FindObjectOfType<QuestDatabase>();
        subQuestDatabase = FindObjectOfType<SubQuestDatabase>();
        questUpdater = GetComponent<QuestUpdater>();
        dataManager = DataManager.Instance;
        Load();
        StartQuest("Tutorial");
        questUpdaterSupport = FindObjectOfType<QuestUpdaterSupport>();
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

    // update quest using name
    public void UpdateQuestStatus(string questName, int index)
    {
        Debug.Log("Quest status update attempt");
        if (!QuestNameIsValid(questName)) { Debug.Log("Not a valid quest name! Check the source is using a proper quest name that is stored in Quest Manager and Quest Database"); return; }
        UpdateQuestHelper(index);
    }

    // update quest using an ID (vs. a name)
    public void UpdateQuestStatus(int questID, int index)
    {
        if (!QuestIDIsValid(questID)) { Debug.Log("Not a valid quest ID! Check the source is using a proper quest ID that is stored in Quest Manager and Quest Database"); return; }
        UpdateQuestHelper(index);
    }

    // decides whether to update a quest given an quest index
    void UpdateQuestHelper(int index)
    {
        // if the quest isn't started or completed, return
        if (!questChecker.isStarted || questChecker.isCompleted) { return; }

        // if the given quest index matches the current quest index, update
        if (questChecker.currentSubQuestIndex == index) 
        {
            // increments quest
            questChecker.subQuests[questChecker.currentSubQuestIndex].isCompleted = true; // update subquest
            questChecker.currentSubQuestIndex++; // go to the next subquest

            // do completion check otherwise run increment updates
            Debug.Log("Task " + questChecker.currentSubQuestIndex + " is complete!");
            if (QuestCompletionCheck(questChecker)) { } // if quest is complete don't need to do anything additional 
            else
            {
                // if the current quest's subquest needs dedicated tracking, turn it on
                if (questChecker.subQuests[questChecker.currentSubQuestIndex].needsDedicatedTracking)
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
