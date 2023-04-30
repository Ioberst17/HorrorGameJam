using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    public bool instantReact;
    private bool playerInRange;
    private bool playerCollision;

    [HideInInspector]public bool isNewExperience;
    public bool destroyObjectAfterUse = true;

    private void Awake()
    {
        playerInRange = false;
    }

    private void Update()
    {
        if (!instantReact)
        {
            if (playerInRange && !DialogueManager.GetInstance().dialogueIsPlaying)
            {
                visualCue.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    DialogueManager.GetInstance().EnterDialogueMode(inkJSON, this.gameObject);
                }
            }
            else { visualCue.SetActive(false); }
        }

        
        if(instantReact && playerCollision && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            if (CheckIfNewWeaponExperience()) { DialogueManager.GetInstance().EnterDialogueMode(inkJSON, this.gameObject); }
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
        if(collider.gameObject.tag == "Player")
        {
            playerInRange = true;
            Debug.Log("Player is in Range");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null) { playerCollision = true; }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            playerInRange = false;
        }
    }
}
