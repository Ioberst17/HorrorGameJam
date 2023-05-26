using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Inventory_UI_Mason : MonoBehaviour
{
    public GameController gameController;
    public DataManager dataManager;
    public NarrativeItemsManager narrativeItemsManager;
    public ConsumablesManager consumablesManager;
    public PlayerData_UI_Mason playerDataUI;
    public WeaponDatabase weaponDatabase;


    [SerializeField]
    public InventoryManager inventoryManager;

    public static int weaponIDNum;
    public string nameOfWeapon;
    public int amountOfAmmo;
    public int weaponLevelNum;


    public int currentNarrativeSlotNum;
    public int currentSecondarySlotNum;
    public int currentConsumableSlotNum;


    [SerializeField] private GameObject _inventory; public GameObject Inventory{ get { return _inventory; } set { _inventory = value; } }

    [SerializeField] private GameObject infoPanel;

    [SerializeField] private Button weaponSideButton;
    [SerializeField] private Button consumableSideButton;
    [SerializeField] private Button narrativeSideButton;

    [SerializeField] private GameObject equipButton;
    [SerializeField] private GameObject useButton;
    [SerializeField] private GameObject xButtonInfo;

    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI weaponName;

    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private TextMeshProUGUI effectAmount;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private TextMeshProUGUI amount;
    

    /*[SerializeField] private TextMeshProUGUI weaponDamage;
    [SerializeField] private TextMeshProUGUI weaponAmmo;
    [SerializeField] private TextMeshProUGUI healingamount;
    [SerializeField] private TextMeshProUGUI amount;
    [SerializeField] private TextMeshProUGUI lucidityAmount;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI healingText;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private TextMeshProUGUI lucidityText;*/



    [SerializeField] private GameObject narrative_Inventory;
    [SerializeField] private GameObject ranged_Inventory;
    [SerializeField] private GameObject consumables_Inventory;


    [SerializeField] private Image inventoryImage;

    private List<GameObject> narrativeSlots = new List<GameObject>();
    private List<GameObject> rangedSlots = new List<GameObject>();
    private List<GameObject> consumableSlots = new List<GameObject>();
    private List<TextMeshProUGUI> rangedAmmoNumbers = new List<TextMeshProUGUI>();
    private List<TextMeshProUGUI> consumableCount = new List<TextMeshProUGUI>();



    [SerializeField] private GameObject narrative_Slot1;
    [SerializeField] private GameObject narrative_Slot2;
    [SerializeField] private GameObject narrative_Slot3;
    [SerializeField] private GameObject narrative_Slot4;
    [SerializeField] private GameObject narrative_Slot5;
    [SerializeField] private GameObject narrative_Slot6;
    [SerializeField] private GameObject narrative_Slot7;
    [SerializeField] private GameObject narrative_Slot8;

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


    public bool InventoryOpen { get; set; }

    private static int ammo;


    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        // initialize the textmeshpro vars to false.
        /*weaponDamage.enabled = false;
        weaponAmmo.enabled = false;
        healingamount.enabled = false;
        amount.enabled = false;
        lucidityAmount.enabled = false;
        description.enabled = false;
        damageText.enabled = false;
        ammoText.enabled = false;
        healingText.enabled = false;
        amountText.enabled = false;
        lucidityText.enabled = false;*/

        effectText.enabled = false;
        effectAmount.enabled = false;
        amountText.enabled = false;
        amount.enabled = false;

        useButton.SetActive(false);
        equipButton.SetActive(false);

        // manually fill lists 
        narrativeSlots.Add(narrative_Slot1);
        narrativeSlots.Add(narrative_Slot2);
        narrativeSlots.Add(narrative_Slot3);
        narrativeSlots.Add(narrative_Slot4);
        narrativeSlots.Add(narrative_Slot5);
        narrativeSlots.Add(narrative_Slot6);
        narrativeSlots.Add(narrative_Slot7);
        narrativeSlots.Add(narrative_Slot8);

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

        //adding in a narrative and consumable item for testing
        narrativeItemsManager.AddItem(1);
        consumablesManager.AddNewItemToInv(1, 1);



        for (int i = 0; i < narrativeSlots.Count; i++)
        {
            narrativeSlots[i].SetActive(false); // hide unused melee weapon slots

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

    public void ToggleUI()
    {
        if (InventoryOpen)
        {
            CloseInventory();
            //Time.timeScale = 1.0f; // resumes time in game
            //if (gameController.isPaused) { gameController.pauseHandler(); }
        }
        else
        {
            OpenInventory();
            //Time.timeScale = 0f; // pauses time game still a slight bug with the player being able to queue attacks and audio still playing when clicking around.
            //if (!gameController.isPaused) { gameController.pauseHandler(); }
        }

        for (int i = 0; i < dataManager.sessionData.narrativeItems.Count; i++)
        {
            narrativeSlots[i].SetActive(true); //turn on inventory slots for melee weapons the player has in their inventory.

            //Debug.Log("weapon=" + dataManager.sessionData.primaryWeapons[i].id + " /n");
        }

        for (int i = 0; i < dataManager.sessionData.secondaryWeapons.Count; i++)
        {
            rangedSlots[i].SetActive(true); //turn on inventory slots for ranged weapons the player has in their inventory.

            rangedAmmoNumbers[i].text = dataManager.sessionData.secondaryWeapons[i].ammo.ToString();

            //Debug.Log(" rangedweapon=" + dataManager.sessionData.secondaryWeapons[i].id + " /n");
        }

        for (int i = 0; i < dataManager.sessionData.consumables.Count; i++)
        {
            consumableSlots[i].SetActive(true); // turn on inventory slots for consumables the player has in their inventory.

            consumableCount[i].text = dataManager.sessionData.consumables[i].amount.ToString(); //update the amount of consumable per item.

            //Debug.Log("consumable=" + dataManager.sessionData.consumables[i].id + " /n");
        }
    }

    private void Update() 
    { 
        if(!gameController.IsPaused && InventoryOpen)
        {
            CloseInventory();
        }
    }


    public void OpenInventory()
    {
        Inventory.SetActive(true);
        InventoryOpen = true;
    }

    public void CloseInventory()
    {
        Inventory.SetActive(false);
        InventoryOpen = false;
    }

    public void OpenMeleeInventory() //switch ui to melee weapons.
    {
        narrative_Inventory.SetActive(true);
        ranged_Inventory.SetActive(false);
        consumables_Inventory.SetActive(false);
    }

    public void OpenRangedInventory() //switch ui to ranged weapons
    {
        narrative_Inventory.SetActive(false);
        ranged_Inventory.SetActive(true);
        consumables_Inventory.SetActive(false);
    }

    public void OpenConsumablesInventory() //switch ui to consumable panel and items.
    {
        narrative_Inventory.SetActive(false);
        ranged_Inventory.SetActive(false);
        consumables_Inventory.SetActive(true);
    }


    public void OpenInfoPanelMelee(int slotNum) //opens the melee weapon panel in the inventory and turns on the text fields and fills them with the info of the current selected weapon.
    {
        infoPanel.SetActive(true);

        useButton.SetActive(false);
        equipButton.SetActive(false);

        DisableSideButtons();

        effectText.enabled = true;
        effectText.text = "Lucidity:";
        effectAmount.enabled = true;

        description.enabled = true;

        currentNarrativeSlotNum = slotNum;

        weaponIDNum = dataManager.sessionData.narrativeItems[slotNum].id;
        weaponName.text = dataManager.sessionData.narrativeItems[slotNum].name;
        effectAmount.text = dataManager.sessionData.narrativeItems[slotNum].amount.ToString();
        description.text = dataManager.sessionData.narrativeItems[slotNum].description;
    }

    public void OpenInfoPanelRanged(int slotNum) //opens the ranged weapon panel in the inventory and turns on the text fields and fills them with the info of the current selected weapon.
    {
        infoPanel.SetActive(true);

        useButton.SetActive(false);
        equipButton.SetActive(true);

        DisableSideButtons();

        effectText.enabled = true;
        effectText.text = "Damage:";
        effectAmount.enabled = true;
        amountText.enabled = true;
        amountText.text = "Ammo:";
        amount.enabled = true;

        description.enabled = true;

        currentSecondarySlotNum = slotNum;

        weaponName.text = dataManager.sessionData.secondaryWeapons[slotNum].name;
        effectAmount.text = weaponDatabase.ReturnItemFromID(slotNum).level1Damage.ToString();
        amount.text = dataManager.sessionData.secondaryWeapons[slotNum].ammo.ToString();
        description.text = dataManager.sessionData.secondaryWeapons[slotNum].description;
    }
    public void OpenInfoPanelConsumable(int slotNum) //opens the consumable panel in the inventory and turns on the text fields and fills them with the info of the current selected item.
    {
        infoPanel.SetActive(true);

        useButton.SetActive(true);
        equipButton.SetActive(false);

        DisableSideButtons();

        effectText.enabled = true;
        effectText.text = "Healing";
        //effectAmount.enabled = true;
        amountText.enabled = true;
        amountText.text = "Amount:";
        amount.enabled = true;

        description.enabled = true;

        currentConsumableSlotNum = slotNum;

        weaponName.text = dataManager.sessionData.consumables[slotNum].itemName;
        amount.text = dataManager.sessionData.consumables[slotNum].amount.ToString();
        //healingamount.text = dataManager.sessionData.consumables[slotNum].amount.ToString();
        description.text = dataManager.sessionData.consumables[slotNum].description;
    }

    public void EquipWeapon()
    {
        //Debug.Log("Current primary weapon: " + dataManager.sessionData.activePrimaryWeapon.name);

        //change the active primary in the datamanger script to the primary weapon that is selected in the inventory
        dataManager.sessionData.activePrimaryWeapon = dataManager.sessionData.secondaryWeapons[currentNarrativeSlotNum].id;

        Debug.Log("New primary weapon: " + dataManager.sessionData.primaryWeapons[currentNarrativeSlotNum].name);

        //change the active secondary in the datamanger script to the secondary weapon that is selected in the inventory
        dataManager.sessionData.activeSecondaryWeapon = dataManager.sessionData.secondaryWeapons[currentSecondarySlotNum].id;

        //run the loadcurrentweapons function from the inventorymanager script to load the newly selected weapons.
        inventoryManager.primaryWeaponsManager.LoadActiveWeapon();
        inventoryManager.secondaryWeaponsManager.LoadActiveWeapon();

        CloseInfoPanel();
    }

    public void UseItem()
    {
        //playerDataUI.UseHealthPack();



        CloseInfoPanel();
    }

    public void CloseInfoPanel() 
    {
        infoPanel.SetActive(false);
        useButton.SetActive(false);
        equipButton.SetActive(false);

        EnableSideButtons();

        effectText.enabled = false;
        effectAmount.enabled = false;
        amountText.enabled = false;
        amount.enabled = false;
        description.enabled = false;
    }

    

    private void DisableSideButtons()
    {
        narrativeSideButton.interactable = false;
        weaponSideButton.interactable = false;
        consumableSideButton.interactable = false;
    }

    private void EnableSideButtons()
    {
        narrativeSideButton.interactable = true;
        weaponSideButton.interactable = true;
        consumableSideButton.interactable = true;
    }

}
