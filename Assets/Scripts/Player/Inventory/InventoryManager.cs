using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class InventoryManager : MonoBehaviour
{
    // DECLARATIONS
    public DataManager dataManager;
    public GameObject utilities;
    public GameObject player;
    public WeaponDatabase weaponDatabase;
    public WeaponDatabase.Database weaponData;
    public ConsumablesDatabase consumablesDatabase;
    public PlayerSecondaryWeapon playerSecondaryWeapon;
    public PlayerHealth playerHealth;


    public ConsumablesManager consumablesManager;
    //[SerializeField]
    //private List<PlayerConsumables> consumables = new List<PlayerConsumables>();

    public PrimaryWeaponsManager primaryWeaponsManager;
    public SecondaryWeaponsManager secondaryWeaponsManager;
    [SerializeField]
    private List<PlayerWeapons> primaryWeapons;
    [SerializeField]
    private List<PlayerWeapons> secondaryWeapons;
    //[SerializeField]
    //private List<PlayerWeapons> primaryWeapons = new List<PlayerWeapons>();
    //[SerializeField]
    //private List<PlayerWeapons> secondaryWeapons = new List<PlayerWeapons>();
    [SerializeField]
    private List<NarrativeItems> narrativeItems = new List<NarrativeItems>();

    private int currentPrimaryWeaponID;
    private int currentPrimaryWeaponIndex;
    [SerializeField]
    private int currentSecondaryWeaponID; // the weapon ID of the current weapon the player is using - in weapon database
    [SerializeField]
    private int currentSecondaryWeaponIndex; // the current weapon location in inventory
    private float lastWeaponUseTime;

    void Start()
    {
        dataManager = DataManager.Instance;
        utilities = GameObject.Find("Utilities");
        weaponDatabase = utilities.GetComponentInChildren<WeaponDatabase>();
        consumablesDatabase = utilities.GetComponentInChildren<ConsumablesDatabase>();
        player = GameObject.Find("Player");
        consumablesManager = GetComponent<ConsumablesManager>();
        primaryWeaponsManager = GetComponent<PrimaryWeaponsManager>();
        primaryWeapons = primaryWeaponsManager.weaponList;
        secondaryWeaponsManager = GetComponent<SecondaryWeaponsManager>();
        secondaryWeapons = secondaryWeaponsManager.weaponList;
        playerHealth = player.GetComponentInChildren<PlayerHealth>();
        playerSecondaryWeapon = player.GetComponentInChildren<PlayerSecondaryWeapon>();
        weaponData = weaponDatabase.weaponDatabase;

        //items
        EventSystem.current.onItemPickupTrigger += AddItem;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) {EventSystem.current.WeaponAddAmmoTrigger(100);}

        if (Input.GetKeyDown(KeyCode.Alpha8)) {EventSystem.current.WeaponChangeTrigger(-1);}

        if (Input.GetKeyDown(KeyCode.Alpha9)){EventSystem.current.WeaponChangeTrigger(1);}

        //if (Input.GetKeyDown(KeyCode.M)){SaveToDataManager();}

        if (Input.GetKeyDown(KeyCode.C))
        {
            consumablesManager.consumables.Clear();
            primaryWeaponsManager.weaponList.Clear();
            secondaryWeaponsManager.weaponList.Clear();
        }

        if (Input.GetKeyDown(KeyCode.J)){ AddItem(0, 10); }
    }

    void AddItem(int itemID, int amount)
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
        Consumables[] consumablesDB = consumablesDatabase.consumablesDatabase.entries;

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
        EventSystem.current.onItemPickupTrigger -= AddItem;
    }
}
