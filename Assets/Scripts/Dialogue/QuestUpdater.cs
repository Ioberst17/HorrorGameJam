using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestUpdater : MonoBehaviour
{
    // this script is used to track quest information across multiple object
    // it is toggled on by quest manager for specific quest subquests
    // when all tasks are completed it will turn off and update back to quest manager for subquest completion
    // by contrast, QuestUpdaterSupport is focused on discrete single events e.g. talk to one person, clear one wave of enemies, enter one location
    [System.Serializable]
    public class DialoguesToTrack
    {
        public string questName;
        public int questIndex;
        public bool isActive;
        public Dialogues[] dialogues;
    }

    [System.Serializable]
    public class Dialogues
    {
        public string npcName;
        public bool hasTalkedTo;
    }

    public DialoguesToTrack cachedDialogueTracker;
    public DialoguesToTrack[] dialoguesToTrack;
    QuestManager questManager;
    DataManager dataManager;

    private void Start()
    {
        questManager = FindObjectOfType<QuestManager>();
        dataManager = DataManager.Instance;
    }

    private DialoguesToTrack FindDialogue(string questName, int subQuestIndex)
    {
        DialoguesToTrack dialogueToFind = System.Array.Find(dialoguesToTrack, (dialogue) =>
        {
            return dialogue.questName == questName && dialogue.questIndex == subQuestIndex;
        });

        if (dialogueToFind != null) { return dialogueToFind; }
        else { Debug.Log("Dialogue not found"); }
        return null;
    }

    public void TurnOnTracking(string questName, int subQuestIndex)
    {
        FindDialogue(questName, subQuestIndex).isActive = true;
    }

    public void UpdateDialogueTracker(string questName, int subQuestIndex, string npcName)
    {
        cachedDialogueTracker = FindDialogue(questName, subQuestIndex);
        if (cachedDialogueTracker.isActive == false) { return; }

        UpdateNPCStatus(cachedDialogueTracker, npcName);
    }

    private void UpdateNPCStatus(DialoguesToTrack trackerToUpdate, string npcName) 
    {
        Dialogues dialogue = System.Array.Find(trackerToUpdate.dialogues, (d) => d.npcName == npcName);

        if (dialogue != null)
        {
            dialogue.hasTalkedTo = true;
            CheckIfDialogueSubQuestIsDone(trackerToUpdate);
        }
        else { Debug.Log("Dialogue not found; no NPC Name match"); }
    }

    private void CheckIfDialogueSubQuestIsDone(DialoguesToTrack trackerToUpdate)
    {
        bool allTalkedTo = trackerToUpdate.dialogues.All(dialogue => dialogue.hasTalkedTo);
        if (allTalkedTo) { TurnOffTracking(trackerToUpdate.questName, trackerToUpdate.questIndex); }
    }

    private void TurnOffTracking(string questName, int subQuestIndex)
    {
        questManager.UpdateQuestStatus(questName, subQuestIndex);
    }
}
