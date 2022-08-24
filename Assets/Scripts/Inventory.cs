using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Inventory : MonoBehaviour
{
    // DECLARATIONS
    public DataManager dataManager;
    public List<PlayerWeapons> inventory = new List<PlayerWeapons>(); // to use as inventory
    public WeaponDatabase weaponDatabase;

    void Start()
    {
        dataManager = DataManager.Instance;
        weaponDatabase = GameObject.Find("WeaponDatabase").GetComponent<WeaponDatabase>();
        LoadFromDataManager();

        //subscribe to important event system updates
        EventSystem.current.onWeaponAmmoTrigger += UpdateAmmo;
        EventSystem.current.onWeaponLevelTrigger += UpdateLevel;

        //for debugging event system
        /*inventory.Add(new PlayerWeapons(0, 1, 0));
        inventory.Add(new PlayerWeapons(1, 1, 0));*/
        Debug.Log(weaponDatabase.weaponDatabase.entries[1].id);
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
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            EventSystem.current.WeaponLevelTrigger(1, 1);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            SaveToDataManager();
            Debug.Log("Trigged");
        }
    }

    void LoadFromDataManager() { inventory = dataManager.gameData.inventory; }

    void SaveToDataManager()
    {
        bool idMatch = false; // used while looping data manager to see if a match is found
        for (int i = 0; i < inventory.Count; i++) // loop through player inventory
        {
            idMatch = false;
            for (int j = 0; j < dataManager.gameData.inventory.Count; j++) //loop through data manager
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
    private void UpdateAmmo(int WeaponID, int ammoChange)
    {
        for (int i = 0; i < inventory.Count; i++)
        { if (inventory[i].weaponID == WeaponID) { inventory[i].weaponAmmo += ammoChange; } }
    }
    private void UpdateLevel(int WeaponID, int levelChange)
    {
        for (int i = 0; i < inventory.Count; i++)
        { if (inventory[i].weaponID == WeaponID) { inventory[i].weaponLevel += levelChange; } }
    }

    private void OnDestroy()
    {
        // unsubscribe from events
        EventSystem.current.onWeaponAmmoTrigger -= UpdateAmmo;
        EventSystem.current.onWeaponLevelTrigger -= UpdateLevel;
    }

}
