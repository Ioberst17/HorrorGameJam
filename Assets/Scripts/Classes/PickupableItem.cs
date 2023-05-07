using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class PickupableItem : MonoBehaviour
{
    
    public int staticID;
    public enum ItemTypeOptions { Consumables, Weapons, NarrativeItems};
    [SerializeField] public ItemTypeOptions itemType;
    public int pickupAmount;
    private bool handleThroughDialogueTriggerAndManager;
    private bool hasBeenPicked;
    public  void Start() { Initialize(); }

    public void Initialize() 
    {
        if(itemType == ItemTypeOptions.Weapons || itemType == ItemTypeOptions.NarrativeItems)
        {
            handleThroughDialogueTriggerAndManager = true;
        }
    }

    public void OnCollisionEnter2D(Collision2D Other)
    {
        if(hasBeenPicked == false)
        {
            if (Other.gameObject.GetComponent<PlayerController>() != null)
            {
                GetComponent<SpriteRenderer>().enabled = false;
                if (!handleThroughDialogueTriggerAndManager) { EventSystem.current.ItemPickupTrigger(this); Destroy(this.gameObject, .5f); }
            }
        }
    }
}
