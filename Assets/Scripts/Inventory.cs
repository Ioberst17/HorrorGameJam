using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Inventory : MonoBehaviour
{
    // DECLARATIONS
    public DataManager dataManager;
    public List<PlayerWeapons> inventory; // to use as inventory
    public WeaponDatabase weaponDatabase; 
    
    void Start()
    {
        dataManager = DataManager.Instance;
        weaponDatabase = GameObject.Find("WeaponDatabase").GetComponent<WeaponDatabase>();
        LoadFromDataManager();
    }

    void Update()
    {
        // check for event of firing weapon to decrement inventory ammo, if so call Update Ammo
        // check for event of pickup / purchase of to increment inventory ammo, if so call Update Ammo

        // check for event on leveling, if so call update level
    }

    void LoadFromDataManager() {inventory = dataManager.gameData.inventory;}

    void SaveToDataManager()
    {
        for (int i = 0; i < inventory.Count; i++) // loop through player inventory
        {
            for (int j = 0; j < dataManager.gameData.inventory.Count; j++) //loop through data manager
            {
                if (inventory[i].weaponID == dataManager.gameData.inventory[j].weaponID) // if the ID's match
                {
                    // then update the entry
                    dataManager.gameData.inventory[j].weaponLevel = inventory[i].weaponLevel;
                    dataManager.gameData.inventory[j].weaponAmmo = inventory[i].weaponAmmo;

                }
                else dataManager.gameData.inventory.Add(inventory[i]); // else, add the entry as a new entry
            }
        }
    }

    void AddNewWeapon(Weapons weapon) // pass in the weapon from the database
    {
        bool isTheWeaponThere = false;
        for (int i = 0; i < inventory.Count; i++) // loop through player inventory
        {
            if(weapon.id == inventory[i].weaponID) { isTheWeaponThere = true; } // check if the inventory has the weapon
        }

        if (isTheWeaponThere == false) // if it doesn't have it after the check
        {
            inventory.Add(new PlayerWeapons(weapon.id, weapon.level, 0)); // then add it at level 1, with 0 ammo
        }
    }

    void UpdateAmmo(int WeaponID, int ammoChange)
    {
        for (int i = 0; i < inventory.Count; i++)
        { if (inventory[i].weaponID == WeaponID) { inventory[i].weaponAmmo += ammoChange; } }
    }
    void UpdateLevel(int WeaponID, int levelChange)
    {
        for (int i = 0; i < inventory.Count; i++)
        { if (inventory[i].weaponID == WeaponID) { inventory[i].weaponAmmo += levelChange; } }
    }
}
