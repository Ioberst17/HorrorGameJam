using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static QuestUpdater;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    public bool instantReact;
    private bool playerInRange;

    public bool isPartOfAQuestActivity;
    [System.Serializable]
    public class QuestInfo
    {
        public string questName;
        public int subQuestIndex;
        public string npcName;
    }

    public QuestInfo[] questInfo;

    [HideInInspector]public bool isNewExperience; 
    public bool destroyObjectAfterUse = true;

    private void Awake()
    {
        playerInRange = false;
    }

    public void PlayerInitiatedDialogue()
    {
        if (playerInRange && !DialogueManager.GetInstance().DialogueIsPlaying)
        {
            DialogueManager.GetInstance().EnterDialogueMode(inkJSON, this.gameObject);
        }
    }

    public bool CheckIfNewWeaponExperience()
    {
        if (GetComponent<PickupableItem>() != null) 
        {
            if (GetComponent<PickupableItem>().itemType == PickupableItem.ItemTypeOptions.Weapons)
            {
                int staticID = GetComponent<PickupableItem>().staticID;
                int isACurrentPrimaryWeap = FindObjectOfType<PrimaryWeaponsManager>().CheckInPrimaryWeapons(staticID); 
                int isACurrentSecondaryWeap = FindObjectOfType<SecondaryWeaponsManager>().CheckInPrimaryWeapons(staticID);

                if (isACurrentPrimaryWeap == -1 && isACurrentSecondaryWeap == -1) 
                { 
                    isNewExperience = true;
                    EventSystem.current.ItemPickupTrigger(this.GetComponent<PickupableItem>());
                    return true; 
                }
            }
        }
        return false;
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Player") { playerInRange = true; visualCue.SetActive(true); }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            if (instantReact && !DialogueManager.GetInstance().DialogueIsPlaying)
            {
                if (CheckIfNewWeaponExperience()) { DialogueManager.GetInstance().EnterDialogueMode(inkJSON, this.gameObject); }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player") { playerInRange = false; visualCue.SetActive(false); }
    }
}
