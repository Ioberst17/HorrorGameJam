using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Timeline.Actions.MenuPriority;

public class WeaponsManager : MonoBehaviour
{
    public enum PrimaryOrSecondary { Primary, Secondary };
    public PrimaryOrSecondary primaryOrSecondary;
    public DataManager dataManager;
    public GameObject utilities;
    public GameObject player;
    public WeaponDatabase weaponDatabase;
    public WeaponDatabase.Database weaponData;
    public PlayerSecondaryWeapon playerSecondaryWeapon;

    [SerializeField]
    public List<PlayerWeapons> weaponList = new List<PlayerWeapons>();
    public List<PlayerWeapons> dataManagerWeaponList = new List<PlayerWeapons>();
    public int activeWeapon;
    public int dataManagerActiveWeapon;
    public int currentWeaponID;
    public int currentWeaponIndex;

    // Start is called before the first frame update
    public void Start()
    {
        EventSystem.current.onWeaponChangeTrigger += WeaponChanged;
        EventSystem.current.onWeaponLevelTrigger += WeaponLevelChange;

        dataManager = DataManager.Instance;
        utilities = GameObject.Find("Utilities");
        weaponDatabase = utilities.GetComponentInChildren<WeaponDatabase>();
        weaponData = weaponDatabase.weaponDatabase;
        InitializeDataManagerReferences();
        Load();

        AddWeaponsForTesting();
    }

    void AddWeaponsForTesting() { for (int i = 0; i < weaponData.entries.Length; i++) { AddWeapon(weaponData.entries[i]); } }

    void InitializeDataManagerReferences()
    {
        if (primaryOrSecondary == PrimaryOrSecondary.Primary) 
        { 
            dataManagerWeaponList = dataManager.gameData.primaryWeapons;
            activeWeapon = dataManager.gameData.activePrimaryWeapon;
        }
        else if (primaryOrSecondary == PrimaryOrSecondary.Secondary) 
        {
            dataManagerWeaponList = dataManager.gameData.secondaryWeapons; 
            activeWeapon = dataManager.gameData.activeSecondaryWeapon;
        }
    }

    private void Load()
    {
        if (dataManagerWeaponList == null) { weaponList = new List<PlayerWeapons>(); }
        else { weaponList = dataManagerWeaponList; }

        currentWeaponID = activeWeapon;

        LoadActiveWeapon();
    }

    private void LoadActiveWeapon()
    {
        if (primaryOrSecondary == PrimaryOrSecondary.Primary)
        {
            if (weaponDatabase.validPrimaryWeaponIDs.Contains(currentWeaponID))
            {
                currentWeaponIndex = GetCurrentWeaponInventoryIndex();
                WeaponUIUpdate();
            }
            else { currentWeaponID = weaponDatabase.validPrimaryWeaponIDs[0]; }
        }

        if (primaryOrSecondary == PrimaryOrSecondary.Secondary)
        {
            if (weaponDatabase.validSecondaryWeaponIDs.Contains(currentWeaponID))
            {
                currentWeaponIndex = GetCurrentWeaponInventoryIndex();
                WeaponUIUpdate();
            }
            else { currentWeaponID = weaponDatabase.validSecondaryWeaponIDs[0]; }
        }
    }

    public void SaveWeapons()
    {
        bool idMatch = false; // used while looping data manager to see if a match is found
        for (int i = 0; i < weaponList.Count; i++) // loop through primary weapons
        {
            idMatch = false;
            for (int j = 0; j < dataManagerWeaponList.Count; j++) //loop through data manager
            {
                if (weaponList[i].id == dataManagerWeaponList[j].id) // if the ID's match while looping through
                {
                    Debug.Log("Weapon ID " + j + " is in save data");
                    // then update the data manager entry
                    dataManagerWeaponList[j].level = weaponList[i].level;
                    if(primaryOrSecondary == PrimaryOrSecondary.Secondary) { dataManagerWeaponList[j].ammo = weaponList[i].ammo; }
                    idMatch = true;
                }
            }
            if (idMatch == false) // if an idMatch wasn't found
            {
                dataManagerWeaponList.Add(weaponList[i]); // then, add the entry as a new entry
            }
        }
    }

    public void AddWeapon(Weapons weapon) // pass in a weapon from the database
    {
        int indexInPrimary = -1;
        int indexInSecondary = -1;

        if (weaponList == null && primaryOrSecondary == PrimaryOrSecondary.Primary) { Debug.Log("Primary Weapons Inventory list is null - check if initialized properly"); }
        else { if (weaponList.Count > 0) { indexInPrimary = weaponList.FindIndex(gameObject => Equals(weapon.id, gameObject.id)); } } // returns -1 if object isn't indexed

        if (weaponList == null && primaryOrSecondary == PrimaryOrSecondary.Secondary) { Debug.Log("Secondary Weapons Inventory list is null - check if initialized properly"); }
        else{ if (weaponList.Count > 0) { indexInSecondary = weaponList.FindIndex(gameObject => Equals(weapon.id, gameObject.id)); }}

        if (indexInPrimary == -1 && indexInSecondary == -1)
        {
            if (weapon.isSecondary == true && primaryOrSecondary == PrimaryOrSecondary.Secondary)
            {
                weaponList.Add(new PlayerWeapons(weapon.title, weapon.id, weapon.level, 0, weapon.isSecondary, weapon.fireRate));
                EventSystem.current.WeaponChangeTrigger(0);
            }
            else if((weapon.isSecondary == false && primaryOrSecondary == PrimaryOrSecondary.Primary))
            { weaponList.Add(new PlayerWeapons(weapon.title, weapon.id, weapon.level, 0, weapon.isSecondary, weapon.fireRate)); }
        }
    }
    public void WeaponLevelChange(int weaponID, int levelChange)
    {
        for (int i = 0; i < weaponList.Count; i++)
        { if (weaponList[i].id == weaponID) { weaponList[i].level += levelChange; } }
    }

    public void WeaponChanged(int weaponChange)
    {
        int weaponLocation = GetCurrentWeaponInventoryIndex();

        if (weaponLocation == -1) { WeaponLocationUpdate(0, 0); } //set to first available weapon
        else { if (weaponChange == 1 || weaponChange == -1) { IncrementInventoryWeapon(weaponChange, weaponLocation); } }
    }

    public void WeaponLocationUpdate(int weaponChange, int weaponLocation)
    {
        currentWeaponID = weaponList[weaponLocation + weaponChange].id;
        currentWeaponIndex = weaponLocation + weaponChange;
    }

    public void WeaponUIUpdate()
    {
        if (weaponList.Count > 0)
        {
            string weaponName = weaponList[currentWeaponIndex].name;
            int weaponAmmo = weaponList[currentWeaponIndex].ammo;

            int weaponID = weaponList[currentWeaponIndex].id;
            int weaponLevel = weaponList[currentWeaponIndex].level;

            EventSystem.current.UpdateWeaponUITrigger(weaponName, weaponAmmo);
            EventSystem.current.UpdatePlayerWeaponTrigger(weaponID, weaponLevel);
        }
    }

    public void IncrementInventoryWeapon(int weaponChange, int weaponLocation)
    {
        // if incrementing inventory would overshoot inventory, then set current weapon to 1st inventory element
        if (weaponLocation + weaponChange > weaponList.Count - 1) { WeaponLocationUpdate(0, 0); }
        // or if decrementing inventory would overshoot inventory, then set current weapon to last inventory element
        else if (weaponLocation + weaponChange < 0) { WeaponLocationUpdate(0, weaponList.Count - 1); }
        else { WeaponLocationUpdate(weaponChange, weaponLocation); }

        WeaponUIUpdate();
    }


    public int GetCurrentWeaponInventoryIndex()
    {
        for (int i = 0; i < weaponList.Count; i++)
        { if (weaponList[i].id == currentWeaponID) { return i; } }

        return -1;
    }

    public string GetCurrentWeaponName() { return weaponList[currentWeaponIndex].name; }

    public void OnDestroy()
    {
        // unsubscribe from events
        EventSystem.current.onWeaponChangeTrigger -= WeaponChanged;
        EventSystem.current.onWeaponLevelTrigger -= WeaponLevelChange;
    }
}
