using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsManager : MonoBehaviour
{
    public enum PrimaryOrSecondary { Primary, Secondary };
    public PrimaryOrSecondary primaryOrSecondary;
    public DataManager dataManager;
    public GameObject utilities;
    public GameObject player;
    public PlayerAnimator animator;
    public WeaponDatabase weaponDatabase;
    public WeaponDatabase.DB weaponData;
    public PlayerSecondaryWeapon playerSecondaryWeapon;

    [SerializeField]
    public List<PlayerWeapons> weaponList = new List<PlayerWeapons>();
    public List<PlayerWeapons> dataManagerWeaponList = new List<PlayerWeapons>();
    public Weapons currentWeapon;
    public int activeWeapon;
    public int dataManagerActiveWeapon;
    private int _currentWeaponID;
    public int currentWeaponID
    {
        get { return _currentWeaponID; }
        set 
        { 
            _currentWeaponID = value;
            currentWeapon = weaponDatabase.ReturnItemFromID(_currentWeaponID); // set the current weapon, whenever the ID changes
        }
    }
    public int currentWeaponIndex;
    public bool ChangingIsBlocked { get; set; }

    // Start is called before the first frame update
    public virtual void Start()
    {
        EventSystem.current.onWeaponChangeTrigger += WeaponChanged;
        EventSystem.current.onWeaponLevelTrigger += WeaponLevelChange;

        dataManager = DataManager.Instance;
        utilities = GameObject.Find("Utilities");
        weaponDatabase = utilities.GetComponentInChildren<WeaponDatabase>();
        weaponData = weaponDatabase.data;
        animator = GetComponentInChildren<PlayerAnimator>(); 
        InitializeDataManagerReferences();
        Load();

        AddWeaponsForTesting();
    }

    void AddWeaponsForTesting() { for (int i = 0; i < weaponData.entries.Length; i++) { AddWeapon(weaponData.entries[i]); } }

    void InitializeDataManagerReferences()
    {
        if (primaryOrSecondary == PrimaryOrSecondary.Primary) 
        { 
            dataManagerWeaponList = dataManager.sessionData.primaryWeapons;
            activeWeapon = dataManager.sessionData.activePrimaryWeapon;
        }
        else if (primaryOrSecondary == PrimaryOrSecondary.Secondary) 
        {
            dataManagerWeaponList = dataManager.sessionData.secondaryWeapons; 
            activeWeapon = dataManager.sessionData.activeSecondaryWeapon;
        }
    }

    public void Load()
    {
        if (dataManagerWeaponList == null) { weaponList = new List<PlayerWeapons>(); }
        else { weaponList = dataManagerWeaponList; }

        //currentWeaponID = dataManager.

        LoadActiveWeapon();
    }

    public void LoadActiveWeapon()
    {
        currentWeaponID = dataManager.sessionData.activeSecondaryWeapon;

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

        indexInPrimary = CheckInPrimaryWeapons(weapon.id);
        indexInSecondary = CheckInSecondaryWeapons(weapon.id);
        
        if (indexInPrimary == -1 && indexInSecondary == -1) { AddNewWeaponExperience(weapon); }
    }
    public int CheckInPrimaryWeapons(int weaponID)
    {
        if (weaponList == null && primaryOrSecondary == PrimaryOrSecondary.Primary) { Debug.Log("Primary Weapons Inventory list is null - check if initialized properly"); }
        else { if (weaponList.Count > 0) { return weaponList.FindIndex(gameObject => Equals(weaponID, gameObject.id)); } } // returns -1 if object isn't indexed
        return -1;
    }

    public int CheckInSecondaryWeapons(int weaponID)
    {
        if (weaponList == null && primaryOrSecondary == PrimaryOrSecondary.Secondary) { Debug.Log("Secondary Weapons Inventory list is null - check if initialized properly"); }
        else { if (weaponList.Count > 0) { return weaponList.FindIndex(gameObject => Equals(weaponID, gameObject.id)); } }
        return -1;
    }

    void AddNewWeaponExperience(Weapons weapon)
    {
        if (weapon.isSecondary == true && primaryOrSecondary == PrimaryOrSecondary.Secondary)
        {
            weaponList.Add(new PlayerWeapons(weapon.name, weapon.id, weapon.level, weapon.description, weapon.ammoLimit, weapon.isSecondary, weapon.fireRate, weapon.ammoLimit, weapon.sprite));
            FindObjectOfType<AudioManager>().PlaySFX(weapon.audioOnAcquisition);
            EventSystem.current.WeaponChangeTrigger(0);
        }
        else if ((weapon.isSecondary == false && primaryOrSecondary == PrimaryOrSecondary.Primary))
        { weaponList.Add(new PlayerWeapons(weapon.name, weapon.id, weapon.level, weapon.description, weapon.ammoLimit, weapon.isSecondary, weapon.fireRate, weapon.ammoLimit, weapon.sprite)); }
    }

    public void AddWeapon(int staticID)
    {
        AddWeapon(weaponDatabase.ReturnItemFromID(staticID));
    }
    public void WeaponLevelChange(int weaponID, int levelChange)
    {
        for (int i = 0; i < weaponList.Count; i++)
        { if (weaponList[i].id == weaponID) { weaponList[i].level += levelChange; } }
    }

    public void WeaponChanged(int weaponChange)
    {
        if (!ChangingIsBlocked) // checks for blockers, e.g. in an active throw
        {
            int weaponLocation = GetCurrentWeaponInventoryIndex();

            if (weaponLocation == -1) { WeaponLocationUpdate(0, 0); } //set to first available weapon
            else { if (weaponChange == 1 || weaponChange == -1) { IncrementInventoryWeapon(weaponChange, weaponLocation); } }
        }
    }

    public void WeaponLocationUpdate(int weaponChange, int weaponLocation)
    {
        currentWeaponID = weaponList[weaponLocation + weaponChange].id;
        currentWeaponIndex = weaponLocation + weaponChange;
    }

    public virtual void WeaponUIUpdate()
    {
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

    public virtual void OnDestroy()
    {
        // unsubscribe from events
        EventSystem.current.onWeaponChangeTrigger -= WeaponChanged;
        EventSystem.current.onWeaponLevelTrigger -= WeaponLevelChange;
    }
}
