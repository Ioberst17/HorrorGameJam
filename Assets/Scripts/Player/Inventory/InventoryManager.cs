using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class InventoryManager : MonoBehaviour // INTENDED TO MANAGE ITEM ACTIVITIES THAT CROSS MULTIPLE PLAYER ITEMS
{
    // DECLARATIONS
    private GameObject utilities;
    private GameObject player;
    private ConsumablesDatabase consumablesDatabase;
    private PlayerSecondaryWeapon playerSecondaryWeapon;
    private PlayerHealth playerHealth;
    private Lucidity lucidity;

    // PLAYER ITEM MANAGERS
    private NarrativeItemsManager narrativeItemsManager;
    private ConsumablesManager consumablesManager;
    public PrimaryWeaponsManager primaryWeaponsManager;
    public SecondaryWeaponsManager secondaryWeaponsManager;

    private List<PlayerWeapons> primaryWeapons;
    private List<PlayerWeapons> secondaryWeapons;

    void Start()
    {
        InitializeUtilities();
        InitializePlayerReferences();


        //items
        EventSystem.current.onItemPickupTrigger += AddConsumableItem;
    }

    private void InitializeUtilities()
    {
        utilities = GameObject.Find("Utilities");
        consumablesDatabase = utilities.GetComponentInChildren<ConsumablesDatabase>();
    }

    private void InitializePlayerReferences()
    {
        player = GameObject.Find("Player");
        // Player 'Inventory items'
        narrativeItemsManager = GetComponent<NarrativeItemsManager>();
        consumablesManager = GetComponent<ConsumablesManager>();
        primaryWeaponsManager = GetComponent<PrimaryWeaponsManager>();
        primaryWeapons = primaryWeaponsManager.weaponList;
        secondaryWeaponsManager = GetComponent<SecondaryWeaponsManager>();
        secondaryWeapons = secondaryWeaponsManager.weaponList;

        // Other player references
        playerHealth = player.GetComponentInChildren<PlayerHealth>();
        lucidity = player.GetComponentInChildren<Lucidity>();
        playerSecondaryWeapon = player.GetComponentInChildren<PlayerSecondaryWeapon>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) {EventSystem.current.WeaponAddAmmoTrigger(100);}

        if (Input.GetKeyDown(KeyCode.Alpha8)) {EventSystem.current.WeaponChangeTrigger(-1);}

        if (Input.GetKeyDown(KeyCode.Alpha9)){EventSystem.current.WeaponChangeTrigger(1);}

        if (Input.GetKeyDown(KeyCode.C))
        {
            narrativeItemsManager.narrativeItems.Clear();
            consumablesManager.consumables.Clear();
            primaryWeaponsManager.weaponList.Clear();
            secondaryWeaponsManager.weaponList.Clear();
        }
    }

    void AddConsumableItem(int itemID, int amount)
    {
        bool isNeither = CheckIfItemIsAmmoOrInstantUse(itemID, amount); 
        if (isNeither == true)
        {
            bool itemInInv = consumablesManager.AddExistingItemToInventory(itemID, amount);
            if (itemInInv == false) { consumablesManager.AddNewItemToInv(itemID, amount);}
        }
    }

    bool CheckIfItemIsAmmoOrInstantUse(int itemID, int amount)
    {
        Consumables[] consumablesDB = consumablesDatabase.data.entries;

        bool isAmmo = false; 
        bool isInstantUse = false;

        for(int i = 0; i < consumablesDB.Length; i++)
        {
            if(itemID == consumablesDB[i].id)
            {
                if (consumablesDB[i].itemType == "Ammo")
                {
                    if(secondaryWeaponsManager.GetCurrentWeaponName() == consumablesDB[i].itemName) { secondaryWeaponsManager.AddAmmo(amount); }
                    else
                    {
                        secondaryWeaponsManager.AddAmmo(consumablesDB[i].itemName, amount);
                        // TO-DO: Placeholder for event firing background inventory UI update (i.e. if ammo is for non-current weapon)
                    }
                    isAmmo = true;  
                }
                if (consumablesDB[i].itemType == "Instant Use")
                {
                    if(consumablesDB[i].itemName == "Heart") { playerHealth.AddHealth(10 * amount); }
                    else if(consumablesDB[i].itemName == "Hourglass") { lucidity.Increase("Hourglass"); }
                    else { Debug.LogFormat("Consumable is of type Instant Use, but it's name ({0}) does not match any in the Consumable DB", consumablesDB[i].itemName); }

                    isInstantUse = true;
                }

                FindObjectOfType<AudioManager>().PlaySFX(consumablesDB[i].audioOnPickup);
            }
        }
        bool isNeither = false;
        if(isAmmo == false && isInstantUse == false) { isNeither = true; }
        return isNeither;
    }

    void OnDestroy()
    {
        EventSystem.current.onItemPickupTrigger -= AddConsumableItem;
    }
}
