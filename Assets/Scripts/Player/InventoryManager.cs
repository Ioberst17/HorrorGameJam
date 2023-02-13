using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEditor.Compilation;

public class InventoryManager : MonoBehaviour
{
    // DECLARATIONS
    public DataManager dataManager;
    public WeaponDatabase weaponDatabase;
    public WeaponDatabase.Database weaponData;
    public PlayerWeapon playerWeapon;

    [SerializeField]
    private List<Consumables> consumables = new List<Consumables>();
    [SerializeField]
    private List<PlayerWeapons> primaryWeapons = new List<PlayerWeapons>();
    [SerializeField]
    private List<PlayerWeapons> secondaryWeapons = new List<PlayerWeapons>();

    private int currentPrimaryWeaponID;
    private int currentPrimaryWeaponLocation;
    [SerializeField]
    private int currentSecondaryWeaponID; // the weapon ID of the current weapon the player is using - in weapon database
    [SerializeField]
    private int currentSecondaryWeaponIndex; // the current weapon location in inventory
    private float lastWeaponUseTime;

    void Start()
    {
        dataManager = DataManager.Instance;
        weaponDatabase = GameObject.Find("WeaponDatabase").GetComponent<WeaponDatabase>();
        weaponData = GameObject.Find("WeaponDatabase").GetComponent<WeaponDatabase>().weaponDatabase;
        playerWeapon = GameObject.Find("Weapon").GetComponent<PlayerWeapon>();

        //subscribe to important weapon updates
        EventSystem.current.onWeaponAddAmmoTrigger += AddAmmo;
        EventSystem.current.onWeaponChangeTrigger += WeaponChanged;
        EventSystem.current.onAmmoCheckTrigger += CanWeaponBeFired;
        EventSystem.current.onWeaponFireTrigger += WeaponFired;
        EventSystem.current.onWeaponLevelTrigger += WeaponLevelChange;

        LoadFromDataManager();

        AddWeaponsForTesting();
    }

    void AddWeaponsForTesting()
    {
        for (int i = 0; i < weaponData.entries.Length; i++)
        {
            AddWeapon(weaponData.entries[i]);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) {EventSystem.current.WeaponAddAmmoTrigger(50);}

        if (Input.GetKeyDown(KeyCode.Alpha8)) {EventSystem.current.WeaponChangeTrigger(-1);}

        if (Input.GetKeyDown(KeyCode.Alpha9)){EventSystem.current.WeaponChangeTrigger(1);}

        if (Input.GetKeyDown(KeyCode.M)){SaveToDataManager();}

        if (Input.GetKeyDown(KeyCode.C))
        {
            consumables.Clear();
            primaryWeapons.Clear();
            secondaryWeapons.Clear();
        }
    }

    void LoadFromDataManager() 
    {
        // add inventory lists
        LoadInventoryLists();
        LoadCurrentWeapons();
    }

    void LoadInventoryLists()
    {
        if(dataManager.gameData.consumables == null) {consumables = new List<Consumables>();}
        else { consumables = dataManager.gameData.consumables; }

        if (dataManager.gameData.primaryWeapons == null) { primaryWeapons = new List<PlayerWeapons>(); }
        else { primaryWeapons = dataManager.gameData.primaryWeapons; }

        if (dataManager.gameData.secondaryWeapons == null) { secondaryWeapons = new List<PlayerWeapons>(); }
        else { secondaryWeapons = dataManager.gameData.secondaryWeapons; }
        
    }

    void LoadCurrentWeapons()
    {
        // check to make sure the current weapon ID is possible
        currentPrimaryWeaponID = dataManager.gameData.activePrimaryWeapon;
        currentSecondaryWeaponID = dataManager.gameData.activeSecondaryWeapon;

        /*if (validPrimaryWeaponIDs.Contains(currentPrimaryWeapon))
        {
            currentPrimaryWeaponLocation = GetTheCurrentWeaponsIndexInInventory();
            WeaponUIUpdate();
        }
        else
        {
            currentPrimaryWeapon = validPrimaryWeaponIDs[0];
            WeaponUIUpdate();
        }*/

        if (weaponDatabase.validSecondaryWeaponIDs.Contains(currentSecondaryWeaponID))
        {
            currentSecondaryWeaponIndex = GetTheCurrentWeaponsIndexInInventory();
            WeaponUIUpdate();
        }
        else { currentSecondaryWeaponID = weaponDatabase.validSecondaryWeaponIDs[0]; }
    }

    void SaveToDataManager()
    {
        SavePrimaryWeapons();
        SaveSecondaryWeapons();
        dataManager.gameData.activePrimaryWeapon = currentPrimaryWeaponID;
        dataManager.gameData.activeSecondaryWeapon = currentSecondaryWeaponID; 
    }

    void SavePrimaryWeapons()
    {
        bool idMatch = false; // used while looping data manager to see if a match is found
        for (int i = 0; i < primaryWeapons.Count; i++) // loop through primary weapons
        {
            idMatch = false;
            for (int j = 0; j < dataManager.gameData.primaryWeapons.Count; j++) //loop through data manager
            {
                if (primaryWeapons[i].ID == dataManager.gameData.primaryWeapons[j].ID) // if the ID's match while looping through
                {
                    Debug.Log("Weapon ID " + j + " is in save data");
                    // then update the data manager entry
                    dataManager.gameData.primaryWeapons[j].level = primaryWeapons[i].level;
                    idMatch = true;
                }
            }
            if (idMatch == false) // if an idMatch wasn't found
            {
                Debug.Log("Weapon ID " + i + " should be added");
                dataManager.gameData.primaryWeapons.Add(primaryWeapons[i]); // then, add the entry as a new entry
            }
        }
    }

    void SaveSecondaryWeapons()
    {
        bool idMatch = false; 
        for (int i = 0; i < secondaryWeapons.Count; i++) 
        {
            idMatch = false;
            for (int j = 0; j < dataManager.gameData.secondaryWeapons.Count; j++) 
            {
                if (secondaryWeapons[i].ID == dataManager.gameData.secondaryWeapons[j].ID) 
                {
                    // then update the data manager entry
                    dataManager.gameData.secondaryWeapons[j].level = secondaryWeapons[i].level;
                    dataManager.gameData.secondaryWeapons[j].ammo = secondaryWeapons[i].ammo;
                    idMatch = true;
                }
            }
            if (idMatch == false) // if an idMatch wasn't found
            {
                dataManager.gameData.secondaryWeapons.Add(secondaryWeapons[i]); // then, add the entry as a new entry
            }
        }
    }

    private void AddWeapon(Weapons weapon) // pass in a weapon from the database
    {
        int indexInPrimary = -1;
        int indexInSecondary = -1;

        if (primaryWeapons == null) { Debug.Log("Primary Weapons Inventory list is null - check if initialized properly"); }
        else
        {
            if (primaryWeapons.Count() > 0)
            {
                indexInPrimary = primaryWeapons.FindIndex(gameObject => Equals(weapon.id, gameObject.ID)); // returns -1 if object isn't indexed
            }
        }

        if (secondaryWeapons == null) { Debug.Log("Secondary Weapons Inventory list is null - check if initialized properly"); }
        else 
            {if(secondaryWeapons.Count() > 0)
                {
                    indexInSecondary = secondaryWeapons.FindIndex(gameObject => Equals(weapon.id, gameObject.ID));
                }
            }
        
        if(indexInPrimary == -1 && indexInSecondary == -1)
        {
            if(weapon.isSecondary == true)
            {
                secondaryWeapons.Add(new PlayerWeapons(weapon.title, weapon.id, weapon.level, 0, weapon.isSecondary, weapon.fireRate));
                EventSystem.current.WeaponChangeTrigger(0);
            }
            else { primaryWeapons.Add(new PlayerWeapons(weapon.title, weapon.id, weapon.level, 0, weapon.isSecondary, weapon.fireRate)); }   
        }
    }

    
    private void AddAmmo(int ammoChange)
    {
        AddAmmo(currentSecondaryWeaponID, ammoChange);
        WeaponUIUpdate();
    } // used for adding to current weapon
    
    private void AddAmmo(int weaponID, int ammoChange)
    {
        for (int i = 0; i < secondaryWeapons.Count; i++)
        { if (secondaryWeapons[i].ID == weaponID) { secondaryWeapons[i].ammo += ammoChange; } }
    } // used when needing to add to a weapon with a specific ID e.g. non-current weapon

    private void WeaponFired(int weaponID, int weaponLevel, int ammoChange, int direction)
    {
        secondaryWeapons[currentSecondaryWeaponIndex].ammo += ammoChange;
        WeaponUIUpdate();
    }
    private void WeaponLevelChange(int weaponID, int levelChange)
    {
        for (int i = 0; i < secondaryWeapons.Count; i++)
        { if (secondaryWeapons[i].ID == weaponID) { secondaryWeapons[i].level += levelChange; } }
    }
    
    private void WeaponChanged(int weaponChange)
    {
        int weaponLocation = GetTheCurrentWeaponsIndexInInventory();

        if(weaponLocation == -1) 
        { 
            Debug.Log("Current Weapon is not in inventory");
            WeaponLocationUpdate(0, 0); //set to first available weapon
        }
        else
        {
            if(weaponChange == 1 || weaponChange == -1) { IncrementInventoryWeapon(weaponChange, weaponLocation); }
            else { Debug.Log("Inventory Increment is not 1 or -1"); }
        }
    }

    private void WeaponLocationUpdate(int weaponChange, int weaponLocation)
    {
        currentSecondaryWeaponID = secondaryWeapons[weaponLocation + weaponChange].ID;
        currentSecondaryWeaponIndex = weaponLocation + weaponChange;
    }

    private void WeaponUIUpdate()
    {
        if(secondaryWeapons.Count > 0)
        {
            string weaponName = secondaryWeapons[currentSecondaryWeaponIndex].name;
            int weaponAmmo = secondaryWeapons[currentSecondaryWeaponIndex].ammo;

            int weaponID = secondaryWeapons[currentSecondaryWeaponIndex].ID;
            int weaponLevel = secondaryWeapons[currentSecondaryWeaponIndex].level;

            EventSystem.current.UpdateWeaponUITrigger(weaponName, weaponAmmo);
            EventSystem.current.UpdatePlayerWeaponTrigger(weaponID, weaponLevel);
        }
    }

    private void IncrementInventoryWeapon(int weaponChange, int weaponLocation)
    {
        // if incrementing inventory would overshoot inventory, then set current weapon to 1st inventory element
        if (weaponLocation + weaponChange > secondaryWeapons.Count - 1) 
        { 
            WeaponLocationUpdate(0,0);
        }
        // or if decrementing inventory would overshoot inventory, then set current weapon to last inventory element
        else if (weaponLocation + weaponChange < 0)
        {
            WeaponLocationUpdate(0, secondaryWeapons.Count - 1);
        }
        else 
        {
            WeaponLocationUpdate(weaponChange, weaponLocation);
        }

        WeaponUIUpdate();
    }
    
    private int GetTheCurrentWeaponsIndexInInventory()
    {
        for (int i = 0; i < secondaryWeapons.Count; i++)
        { if (secondaryWeapons[i].ID == currentSecondaryWeaponID) { return i; } }

        return -1;
    }

    private void CanWeaponBeFired(int fireDirection) // used as a check before firing a weapon and decrementing inventory
    {
        bool hasAmmo = secondaryWeapons[currentSecondaryWeaponIndex].ammo > 0;
        bool doesNotExceedFireRate = Time.time > lastWeaponUseTime + secondaryWeapons[currentSecondaryWeaponIndex].fireRate;
        bool canThrow = !playerWeapon.inActiveThrow;

        if (hasAmmo && doesNotExceedFireRate && canThrow) 
        {
            lastWeaponUseTime = Time.time;

            EventSystem.current.WeaponFireTrigger(
                currentSecondaryWeaponID,
                secondaryWeapons[currentSecondaryWeaponIndex].level, 
                -1, 
                fireDirection); // send the weapon fire 
        }         
    }

    private void OnDestroy()
    {
        // unsubscribe from events
        EventSystem.current.onWeaponAddAmmoTrigger -= AddAmmo;
        EventSystem.current.onWeaponChangeTrigger -= WeaponChanged;
        EventSystem.current.onAmmoCheckTrigger -= CanWeaponBeFired;
        EventSystem.current.onWeaponFireTrigger -= WeaponFired;
        EventSystem.current.onWeaponLevelTrigger -= WeaponLevelChange;
    }

}
