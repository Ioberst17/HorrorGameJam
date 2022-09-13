using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Inventory : MonoBehaviour
{
    // DECLARATIONS
    public DataManager dataManager;
    [SerializeField]
    private List<PlayerWeapons> inventory = new List<PlayerWeapons>(); // to use as inventory
    public WeaponDatabase weaponDatabase;
    [SerializeField]
    private int currentWeapon;

    void Start()
    {
        dataManager = DataManager.Instance;
        weaponDatabase = GameObject.Find("WeaponDatabase").GetComponent<WeaponDatabase>();
        

        //subscribe to important weapon updates
        EventSystem.current.onWeaponAddAmmoTrigger += AddAmmo;
        EventSystem.current.onWeaponChangeTrigger += WeaponChanged;
        EventSystem.current.onAmmoCheckTrigger += DoesCurrentWeaponHaveAmmo;
        EventSystem.current.onWeaponFireTrigger += WeaponFired;
        EventSystem.current.onWeaponLevelTrigger += WeaponLevel;

        LoadFromDataManager();

        //for debugging event system
        AddNewWeapon(weaponDatabase.weaponDatabase.entries[0]); //add the weapon at the 1st index
        AddNewWeapon(weaponDatabase.weaponDatabase.entries[1]); //add the weapon at the 1st index
        AddNewWeapon(weaponDatabase.weaponDatabase.entries[2]); //add the weapon at the 1st index
        AddNewWeapon(weaponDatabase.weaponDatabase.entries[3]); //add the weapon at the 1st index
        
    }

    void Update()
    {
        // check for event of firing weapon to decrement inventory ammo, if so call Update Ammo
        // check for event of pickup / purchase of to increment inventory ammo, if so call Update Ammo

        // check for event on leveling, if so call update level
        /*if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            EventSystem.current.WeaponAmmoTrigger(1, 1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            EventSystem.current.WeaponAmmoTrigger(1, -1);
        }*/


        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            EventSystem.current.WeaponAddAmmoTrigger(1, 1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            EventSystem.current.WeaponAddAmmoTrigger(2, 1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            EventSystem.current.WeaponChangeTrigger(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            EventSystem.current.WeaponChangeTrigger(2);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            SaveToDataManager();
        }
    }

    void LoadFromDataManager() 
    {
        currentWeapon = dataManager.gameData.activeWeapon;
        inventory = dataManager.gameData.inventory; 
    }

    void SaveToDataManager()
    {
        bool idMatch = false; // used while looping data manager to see if a match is found
        for (int i = 0; i < inventory.Count; i++) // loop through player inventory
        {
            idMatch = false;
            for (int j = 0; j < dataManager.gameData.inventory.Count; j++) //loop through data manager and update entires
            {
                if (inventory[i].weaponID == dataManager.gameData.inventory[j].weaponID) // if the ID's match while looping through
                {
                    // then update the data manager entry
                    dataManager.gameData.inventory[j].weaponLevel = inventory[i].weaponLevel;
                    dataManager.gameData.inventory[j].weaponAmmo = inventory[i].weaponAmmo;
                    idMatch = true;
                }
            }
            if (idMatch == false) // if an idMatch wasn't found
            {
                dataManager.gameData.inventory.Add(inventory[i]); // then, add the entry as a new entry
            }
        }
        dataManager.gameData.activeWeapon = currentWeapon; // save the current active weapon's ID

    }

    private void AddNewWeapon(Weapons weapon) // pass in a weapon from the database
    {
        bool isTheWeaponThere = false; // assume the weapon isn't there
        for (int i = 0; i < inventory.Count; i++) // loop through player inventory
        {
            if (weapon.id == inventory[i].weaponID) { isTheWeaponThere = true; } // check if the inventory has the weapon
        }

        if (isTheWeaponThere == false) // if it doesn't have it after the check
        {
            inventory.Add(new PlayerWeapons(weapon.id, weapon.level, 0)); // then add it at level 1, with 0 ammo
        }
    }

    private void AddAmmo(int weaponID, int ammoChange)
    {
        for (int i = 0; i < inventory.Count; i++)
        { if (inventory[i].weaponID == weaponID) { inventory[i].weaponAmmo += ammoChange; } }
    }

    private void WeaponFired(int weaponID, int weaponLevel, int ammoChange, int direction)
    {
        for (int i = 0; i < inventory.Count; i++)
        { if (inventory[i].weaponID == weaponID) { inventory[i].weaponAmmo += ammoChange; } }
    }
    private void WeaponLevel(int weaponID, int levelChange)
    {
        for (int i = 0; i < inventory.Count; i++)
        { if (inventory[i].weaponID == weaponID) { inventory[i].weaponLevel += levelChange; } }
    }
    
    private void WeaponChanged(int weaponID)
    {
        currentWeapon = weaponID;
    }
    
    private void DoesCurrentWeaponHaveAmmo()
    {
        for (int i = 0; i < inventory.Count; i++) // loop through inventory
        { if (inventory[i].weaponID == currentWeapon) // if the loop finds the current weapon 
            { if(inventory[i].weaponAmmo > 0) // and if the current weapon has ammo
                EventSystem.current.WeaponFireTrigger(currentWeapon,inventory[i].weaponLevel, -1, 0); ; // send the weapon fire event
            } 
        } 
    }

    private void OnDestroy()
    {
        // unsubscribe from events
        EventSystem.current.onWeaponAddAmmoTrigger -= AddAmmo;
        EventSystem.current.onWeaponChangeTrigger -= WeaponChanged;
        EventSystem.current.onAmmoCheckTrigger += DoesCurrentWeaponHaveAmmo;
        EventSystem.current.onWeaponFireTrigger -= WeaponFired;
        EventSystem.current.onWeaponLevelTrigger -= WeaponLevel;
    }

}
