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
    private int currentWeapon; // the weapon ID of the current weapon the player is using - in weapon database
    private int currentWeaponLocation; // the current weapon location in inventory

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
        AddNewWeapon(weaponDatabase.weaponDatabase.entries[0]); 
        AddNewWeapon(weaponDatabase.weaponDatabase.entries[1]); 
        AddNewWeapon(weaponDatabase.weaponDatabase.entries[2]); 
        AddNewWeapon(weaponDatabase.weaponDatabase.entries[3]); 
        AddNewWeapon(weaponDatabase.weaponDatabase.entries[4]);
        AddNewWeapon(weaponDatabase.weaponDatabase.entries[5]);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            EventSystem.current.WeaponAddAmmoTrigger(10);
        }

        if (Input.GetKeyDown(KeyCode.Alpha8)) // decrement to the lower weapon in player inventory
        {
            
            EventSystem.current.WeaponChangeTrigger(-1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            EventSystem.current.WeaponChangeTrigger(1);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            SaveToDataManager();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            inventory.Clear();
        }
    }

    void LoadFromDataManager() 
    {
        inventory = dataManager.gameData.inventory;
        currentWeapon = dataManager.gameData.activeWeapon;
        currentWeaponLocation = GetTheCurrentWeaponsIndexInInventory();
        WeaponUIUpdate();
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
            inventory.Add(new PlayerWeapons(weapon.title, weapon.id, weapon.level, 0)); // then add it at level 1, with 0 ammo, at the back of inventory
            // and set it as the current weapon
            WeaponLocationUpdate(0, inventory.Count - 1);
            // update UI
            WeaponUIUpdate();
        }
    }

    
    private void AddAmmo(int ammoChange)
    {
        AddAmmo(currentWeapon, ammoChange);
        WeaponUIUpdate();
    } // used for adding to current weapon
    
    private void AddAmmo(int weaponID, int ammoChange)
    {
        for (int i = 0; i < inventory.Count; i++)
        { if (inventory[i].weaponID == weaponID) { inventory[i].weaponAmmo += ammoChange; } }
    } // used when needing to add to a weapon with a specific ID e.g. non-current weapon

    private void WeaponFired(int weaponID, int weaponLevel, int ammoChange, int direction)
    {
        inventory[currentWeaponLocation].weaponAmmo += ammoChange;
        WeaponUIUpdate();
    }
    private void WeaponLevel(int weaponID, int levelChange)
    {
        for (int i = 0; i < inventory.Count; i++)
        { if (inventory[i].weaponID == weaponID) { inventory[i].weaponLevel += levelChange; } }
    }
    
    private void WeaponChanged(int weaponChange)
    {
        int weaponLocation = GetTheCurrentWeaponsIndexInInventory();

        if(weaponLocation == -1) { Debug.Log("Current Weapon is not in inventory"); }
        else
        {
            if(weaponChange == 1 || weaponChange == -1) { IncrementInventoryWeapon(weaponChange, weaponLocation); }
            else { Debug.Log("Inventory Increment is not 1 or -1"); }
        }
    }

    private void WeaponLocationUpdate(int weaponChange, int weaponLocation)
    {
        currentWeapon = inventory[weaponLocation + weaponChange].weaponID;
        currentWeaponLocation = weaponLocation + weaponChange;
    }

    private void WeaponUIUpdate()
    {
        string weaponName = inventory[currentWeaponLocation].weaponName;
        int weaponAmmo = inventory[currentWeaponLocation].weaponAmmo;

        EventSystem.current.UpdateAmmoUITrigger(weaponName, weaponAmmo);
    }

    private void IncrementInventoryWeapon(int weaponChange, int weaponLocation)
    {
        // if incrementing inventory would overshoot inventory, then set current weapon to 1st inventory element
        if (weaponLocation + weaponChange > inventory.Count - 1) 
        { 
            WeaponLocationUpdate(0,0);
        }
        // or if decrementing inventory would overshoot inventory, then set current weapon to last inventory element
        else if (weaponLocation + weaponChange < 0)
        {
            WeaponLocationUpdate(0, inventory.Count - 1);
        }
        else 
        {
            WeaponLocationUpdate(weaponChange, weaponLocation);
        }

        WeaponUIUpdate();
    }
    
    private int GetTheCurrentWeaponsIndexInInventory()
    {
        for (int i = 0; i < inventory.Count; i++)
        { if (inventory[i].weaponID == currentWeapon) { return i; } }

        return -1;
    }

    private void DoesCurrentWeaponHaveAmmo(int fireDirection) // used as a check before firing a weapon and decrementing inventory
    {
        for (int i = 0; i < inventory.Count; i++) // loop through inventory
        { if (inventory[i].weaponID == currentWeapon) // if the loop finds the current weapon 
            { if(inventory[i].weaponAmmo > 0) // and if the current weapon has ammo
                EventSystem.current.WeaponFireTrigger(currentWeapon,inventory[i].weaponLevel, -1, fireDirection); // send the weapon fire event
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
