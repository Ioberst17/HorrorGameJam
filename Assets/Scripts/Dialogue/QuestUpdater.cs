using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Ink.Runtime;
using System.Security.Cryptography;

public class QuestUpdater : MonoBehaviour
{
    // this script is used to track quest information across multiple object
    // it is toggled on by quest manager for specific quest subquests
    // when all tasks are completed it will turn off and update back to quest manager for subquest completion
    // by contrast, QuestUpdaterSupport is focused on discrete single events e.g. talk to one person, clear one wave of enemies, enter one location
    
    // a tracker, there are a few of this in the array instance; each of these is a 'sub-quest'
    [System.Serializable]
    public class DialogueTracker
    {
        [Header("Identifier")]
        public string questName;
        public int questIndex;

        [Header("Progress")]
        public bool isActive;
        // tracked for UI / other systems to reference
        public int numOfSubTasksCompleted; // captures the actual number of tasks accomplished of the total
        public float percentOfTaskComplete; // captures the number of tasks : total tasks

        // actual set of dialogues to track status of
        public Dialogues[] dialogues;
    }

    // an individual item to track
    [System.Serializable]
    public class Dialogues
    {
        public string npcName;
        public bool hasTalkedTo;
    }

    public DialogueTracker[] dialoguesToTrack; // an array of trackers
    public DialogueTracker cachedDialogueTracker; // used to reference in memory vs creating a temp variable
    
    // external references
    QuestManager questManager;
    DataManager dataManager;

    private void Start()
    {
        questManager = FindObjectOfType<QuestManager>();
        dataManager = DataManager.Instance;
    }

    // gets a specific dialogue tracker from the array that tracks based on the quest name and the sub-quest number e.g.
    // "Get's the subquest of index 2 on the "Tutorial" quest, that involves talking to a few NPCs 
    private DialogueTracker FindDialogue(string questName, int subQuestIndex)
    {
        DialogueTracker dialogueToFind = System.Array.Find(dialoguesToTrack, (dialogue) =>
        {
            return dialogue.questName == questName && dialogue.questIndex == subQuestIndex;
        });

        if (dialogueToFind != null) { return dialogueToFind; }
        else { Debug.Log("Dialogue not found"); }
        return null;
    }

    // used to know when to start tracking an activity i.e. dialogue
    public void TurnOnTracking(string questName, int subQuestIndex) { FindDialogue(questName, subQuestIndex).isActive = true; }

    // updates dialogue tracker if the quest is active
    public void UpdateDialogueTracker(string questName, int subQuestIndex, string npcName)
    {
        cachedDialogueTracker = FindDialogue(questName, subQuestIndex);
        if (cachedDialogueTracker.isActive == false) { return; }

        UpdateNPCStatus(cachedDialogueTracker, npcName);
    }

    // check for the npc name in the dialogue tracker, and if so do updates
    private void UpdateNPCStatus(DialogueTracker trackerToUpdate, string npcName) 
    {
        Dialogues dialogue = System.Array.Find(trackerToUpdate.dialogues, (d) => d.npcName == npcName);

        if (dialogue != null)
        {
            if(dialogue.hasTalkedTo != true)
            {
                dialogue.hasTalkedTo = true;
                trackerToUpdate.numOfSubTasksCompleted++;
                trackerToUpdate.percentOfTaskComplete = trackerToUpdate.numOfSubTasksCompleted / trackerToUpdate.dialogues.Length;
                CheckSubQuestCompletion(trackerToUpdate);
            }
        }
        else { Debug.Log("Dialogue not found; no NPC Name match"); }
    }

    private void CheckSubQuestCompletion(DialogueTracker trackerToUpdate)
    {
        bool allTalkedTo = trackerToUpdate.dialogues.All(dialogue => dialogue.hasTalkedTo);
        if (allTalkedTo) { TurnOffTracking(trackerToUpdate.questName, trackerToUpdate.questIndex); }
    }

    // called when subquest is complete to turn off tracking in questManager
    private void TurnOffTracking(string questName, int subQuestIndex) { questManager.UpdateQuestStatus(questName, subQuestIndex); }
}
