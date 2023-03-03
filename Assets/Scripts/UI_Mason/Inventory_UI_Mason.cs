using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Inventory_UI_Mason : MonoBehaviour
{

    public DataManager dataManager;

    
    public PlayerData_UI_Mason playerDataUI;

    [SerializeField]
    public InventoryManager inventoryManager;

    public static int weaponIDNum;
    public string nameOfWeapon;
    public int amountOfAmmo;
    public int weaponLevelNum;
    public int currentSlotNum;


    [SerializeField] private GameObject inventory;

    [SerializeField] private GameObject infoPanel;

    [SerializeField] private TextMeshProUGUI weaponName;
    [SerializeField] private TextMeshProUGUI weaponDamage;
    [SerializeField] private TextMeshProUGUI weaponAmmo;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI ammoText;

    [SerializeField] private GameObject xButtonInfo;

    [SerializeField] private GameObject melee_Inventory;
    [SerializeField] private GameObject ranged_Inventory;
    [SerializeField] private GameObject consumables_Inventory;


    [SerializeField] private Image inventoryImage;

    private List<GameObject> meleeSlots = new List<GameObject>();
    private List<GameObject> rangedSlots = new List<GameObject>();
    private List<GameObject> consumableSlots = new List<GameObject>();
    private List<TextMeshProUGUI> rangedAmmoNumbers = new List<TextMeshProUGUI>();
    private List<TextMeshProUGUI> consumableCount = new List<TextMeshProUGUI>();



    [SerializeField] private GameObject melee_Slot1;
    [SerializeField] private GameObject melee_Slot2;
    [SerializeField] private GameObject melee_Slot3;
    [SerializeField] private GameObject melee_Slot4;
    [SerializeField] private GameObject melee_Slot5;

    [SerializeField] private GameObject ranged_Slot1;
    [SerializeField] private TextMeshProUGUI r_Ammo1;

    [SerializeField] private GameObject ranged_Slot2;
    [SerializeField] private TextMeshProUGUI r_Ammo2;

    [SerializeField] private GameObject ranged_Slot3;
    [SerializeField] private TextMeshProUGUI r_Ammo3;

    [SerializeField] private GameObject ranged_Slot4;
    [SerializeField] private TextMeshProUGUI r_Ammo4;

    [SerializeField] private GameObject ranged_Slot5;
    [SerializeField] private TextMeshProUGUI r_Ammo5;

    [SerializeField] private GameObject ranged_Slot6;
    [SerializeField] private TextMeshProUGUI r_Ammo6;

    [SerializeField] private GameObject consumables_Slot1;
    [SerializeField] private TextMeshProUGUI consumable_count_slot1;

    [SerializeField] private GameObject consumables_Slot2;
    [SerializeField] private TextMeshProUGUI consumable_count_slot2;


    public static bool inventoryOpen = false;

    private static int ammo;


    // Start is called before the first frame update
    void Start()
    {
        // initialize the textmeshpro vars to false.
        weaponDamage.enabled = false;
        weaponAmmo.enabled = false;
        description.enabled = false;
        damageText.enabled = false;
        ammoText.enabled = false;

        // manually fill lists 
        meleeSlots.Add(melee_Slot1);
        meleeSlots.Add(melee_Slot2);
        meleeSlots.Add(melee_Slot3);
        meleeSlots.Add(melee_Slot4);
        meleeSlots.Add(melee_Slot5);

        rangedSlots.Add(ranged_Slot1);
        rangedSlots.Add(ranged_Slot2);
        rangedSlots.Add(ranged_Slot3);
        rangedSlots.Add(ranged_Slot4);
        rangedSlots.Add(ranged_Slot5);
        rangedSlots.Add(ranged_Slot6);

        consumableSlots.Add(consumables_Slot1);
        consumableSlots.Add(consumables_Slot2);

        rangedAmmoNumbers.Add(r_Ammo1);
        rangedAmmoNumbers.Add(r_Ammo2);
        rangedAmmoNumbers.Add(r_Ammo3);
        rangedAmmoNumbers.Add(r_Ammo4);
        rangedAmmoNumbers.Add(r_Ammo5);
        rangedAmmoNumbers.Add(r_Ammo6);

        consumableCount.Add(consumable_count_slot1);
        consumableCount.Add(consumable_count_slot2);


        for (int i = 0; i < meleeSlots.Count; i++)
        {
            meleeSlots[i].SetActive(false); // hide unused melee weapon slots

            //Debug.Log("melee slots hidden.\n");
        }

        for (int i = 0; i < rangedSlots.Count; i++)
        {
            rangedSlots[i].SetActive(false); // hide unused ranged weapon slots
            //Debug.Log("ranged slots hidden.\n");
        }

        for (int i = 0; i < consumableSlots.Count; i++)
        {
            consumableSlots[i].SetActive(false); // hide unused consumable slots
            //Debug.Log("consumable slots hidden.\n");
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inventoryOpen)
            {
                CloseInventory();
                Time.timeScale = 1.0f; // resumes time in game
            }
            else
            {
                OpenInventory();
                Time.timeScale = 0f; // pauses time game still a slight bug with the player being able to queue attacks and audio still playing when clicking around.
            }

            for (int i = 0; i < dataManager.gameData.primaryWeapons.Count; i++)
            {
                meleeSlots[i].SetActive(true); //turn on inventory slots for melee weapons the player has in their inventory.

                //Debug.Log("weapon=" + dataManager.gameData.primaryWeapons[i].id + " /n");
            }

            for (int i = 0; i < dataManager.gameData.secondaryWeapons.Count; i++)
            {
                rangedSlots[i].SetActive(true); //turn on inventory slots for ranged weapons the player has in their inventory.

                rangedAmmoNumbers[i].text = dataManager.gameData.secondaryWeapons[i].ammo.ToString();

                //Debug.Log(" rangedweapon=" + dataManager.gameData.secondaryWeapons[i].id + " /n");
            }

            for (int i = 0; i < dataManager.gameData.consumables.Count; i++)
            {
                consumableSlots[i].SetActive(true); // turn on inventory slots for consumables the player has in their inventory.

                consumableCount[i].text = dataManager.gameData.consumables[i].amount.ToString(); //update the amount of consumable per item.

                //Debug.Log("consumable=" + dataManager.gameData.consumables[i].id + " /n");
            }

            
        }
    }


    public void OpenInventory()
    {
        inventory.SetActive(true);
        inventoryOpen = true;
    }

    public void CloseInventory()
    {
        inventory.SetActive(false);
        inventoryOpen = false;
    }

    public void OpenMeleeInventory() //switch ui to melee weapons.
    {
        melee_Inventory.SetActive(true);
        ranged_Inventory.SetActive(false);
        consumables_Inventory.SetActive(false);
    }

    public void OpenRangedInventory() //switch ui to ranged weapons
    {
        melee_Inventory.SetActive(false);
        ranged_Inventory.SetActive(true);
        consumables_Inventory.SetActive(false);
    }

    public void OpenConsumablesInventory() //switch ui to consumable panel and items.
    {
        melee_Inventory.SetActive(false);
        ranged_Inventory.SetActive(false);
        consumables_Inventory.SetActive(true);
    }


    public void OpenInfoPanelMelee(int slotNum) //opens the melee weapon panel in the inventory and turns on the text fields and fills them with the info of the current selected weapon.
    {
        infoPanel.SetActive(true);
        weaponDamage.enabled = true;
        damageText.enabled = true;

        currentSlotNum = slotNum;

        weaponIDNum = dataManager.gameData.primaryWeapons[slotNum].id;
        nameOfWeapon = dataManager.gameData.primaryWeapons[slotNum].name;
        weaponName.text = dataManager.gameData.primaryWeapons[slotNum].name;
        weaponDamage.text = dataManager.gameData.primaryWeapons[slotNum].level.ToString();
    }
    public void OpenInfoPanelRanged(int slotNum) //opens the ranged weapon panel in the inventory and turns on the text fields and fills them with the info of the current selected weapon.
    {
        infoPanel.SetActive(true);
        weaponDamage.enabled = true;
        damageText.enabled = true;
        ammoText.enabled = true;
        weaponAmmo.enabled = true;

        currentSlotNum = slotNum;

        weaponName.text = dataManager.gameData.secondaryWeapons[slotNum].name;
        weaponDamage.text = dataManager.gameData.secondaryWeapons[slotNum].level.ToString();
        weaponAmmo.text = dataManager.gameData.secondaryWeapons[slotNum].ammo.ToString();
    }
    public void OpenInfoPanelConsumable(int slotNum) //opens the consumable panel in the inventory and turns on the text fields and fills them with the info of the current selected item.
    {
        infoPanel.SetActive(true);
        description.enabled = true;
        ammoText.enabled = true;
        weaponAmmo.enabled = true;

        currentSlotNum = slotNum;

        weaponName.text = dataManager.gameData.consumables[slotNum].itemName;
        weaponAmmo.text = dataManager.gameData.consumables[slotNum].amount.ToString();
        description.text = dataManager.gameData.consumables[slotNum].description;
    }

    public void CloseInfoPanel() //just close the info panel. assigned to the equip button.
    {
        infoPanel.SetActive(false);
        weaponDamage.enabled = false;
        damageText.enabled = false;
        ammoText.enabled = false;
        weaponAmmo.enabled = false;
        description.enabled = false;
    }

    public void EquipWeapon()
    {
        //change the active secondary in the datamanger script to the secondary weapon that is selected in the inventory
        dataManager.gameData.activeSecondaryWeapon = dataManager.gameData.secondaryWeapons[currentSlotNum].id;

        //run the loadcurrentweapons function from the inventorymanager script to load the newly selected weapons.
        inventoryManager.LoadCurrentWeapons();

        infoPanel.SetActive(false); // turn off info panel
    }

}
