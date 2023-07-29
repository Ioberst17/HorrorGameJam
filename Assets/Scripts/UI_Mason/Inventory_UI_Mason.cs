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
    private int currentObjectiveSlotNum;


    [SerializeField] private GameObject _inventory; public GameObject Inventory{ get { return _inventory; } set { _inventory = value; } }

    [SerializeField] private GameObject infoPanel;

    [SerializeField] private Button weaponTab;
    [SerializeField] private Button consumableTab;
    [SerializeField] private Button narrativeTab;
    [SerializeField] private Button objectiveTab;

    [SerializeField] private GameObject equipButton;
    [SerializeField] private GameObject useButton;
    [SerializeField] private GameObject xButtonInfo;

    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI weaponName;
    [SerializeField] private TextMeshProUGUI equippedText;

    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private TextMeshProUGUI effectAmount;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private TextMeshProUGUI amount;

    [SerializeField] private TextMeshProUGUI objectiveTitle;
    [SerializeField] private TextMeshProUGUI currentObjectiveStepText;
    [SerializeField] private TextMeshProUGUI currentCompletedStepsText;
    [SerializeField] private TextMeshProUGUI currentHeaderText;
    [SerializeField] private TextMeshProUGUI completedHeaderText;
    [SerializeField] private TextMeshProUGUI objectiveDescription;

    [SerializeField] private GameObject narrative_Inventory;
    [SerializeField] private GameObject ranged_Inventory;
    [SerializeField] private GameObject consumables_Inventory;
    [SerializeField] private GameObject currentObjective_Inventory;
    [SerializeField] private GameObject completedObjective_Inventory;

    private List<GameObject> narrativeSlots = new List<GameObject>();
    private List<GameObject> currentObjectiveSlots = new List<GameObject>();
    private List<GameObject> completedObjectiveSlots = new List<GameObject>();
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

    [SerializeField] private GameObject currentObjective_Slot1;

    [SerializeField] private GameObject completedObjective_Slot1;

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

        effectText.enabled = false;
        effectAmount.enabled = false;
        amountText.enabled = false;
        amount.enabled = false;

        equippedText.enabled = false;

        useButton.SetActive(false);
        equipButton.SetActive(false);

        weaponTab.interactable = true;
        consumableTab.interactable = true;
        narrativeTab.interactable = true;

        // manually fill lists 
        narrativeSlots.Add(narrative_Slot1);
        narrativeSlots.Add(narrative_Slot2);
        narrativeSlots.Add(narrative_Slot3);
        narrativeSlots.Add(narrative_Slot4);
        narrativeSlots.Add(narrative_Slot5);
        narrativeSlots.Add(narrative_Slot6);
        narrativeSlots.Add(narrative_Slot7);
        narrativeSlots.Add(narrative_Slot8);

        currentObjectiveSlots.Add(currentObjective_Slot1);

        completedObjectiveSlots.Add(completedObjective_Slot1);

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
        //narrativeItemsManager.AddItem(1);
        //consumablesManager.AddNewItemToInv(1, 1);



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

        //Debug.Log("current weapon id is: " + dataManager.sessionData.secondaryWeapons[dataManager.sessionData.activeSecondaryWeapon].id.ToString() + "\n");

    }


    public void OpenInventory()
    {
        Inventory.SetActive(true);
        InventoryOpen = true;

        OpenInfoPanelWeapon(dataManager.sessionData.secondaryWeapons[dataManager.sessionData.activeSecondaryWeapon].id);

        //EnableSideButtons();
    }

    public void CloseInventory()
    {
        Inventory.SetActive(false);
        InventoryOpen = false;
    }

    public void OpenNarrativeItemInventory() //switch ui to melee weapons.
    {
        Debug.Log("Weapons Button Pressed!!!\n");
        narrative_Inventory.SetActive(true);
        ranged_Inventory.SetActive(false);
        consumables_Inventory.SetActive(false);
        currentObjective_Inventory.SetActive(false);
        completedObjective_Inventory.SetActive(false);
    }

    public void OpenRangedInventory() //switch ui to ranged weapons
    {
        narrative_Inventory.SetActive(false);
        ranged_Inventory.SetActive(true);
        consumables_Inventory.SetActive(false);
        currentObjective_Inventory.SetActive(false);
        completedObjective_Inventory.SetActive(false);
    }

    public void OpenConsumablesInventory() //switch ui to consumable panel and items.
    {
        narrative_Inventory.SetActive(false);
        ranged_Inventory.SetActive(false);
        consumables_Inventory.SetActive(true);
        currentObjective_Inventory.SetActive(false);
        completedObjective_Inventory.SetActive(false);
    }

    public void OpenObjectiveInventory() //switch ui to objective panel and items.
    {
        narrative_Inventory.SetActive(false);
        ranged_Inventory.SetActive(false);
        consumables_Inventory.SetActive(false);
        currentObjective_Inventory.SetActive(true);
        completedObjective_Inventory.SetActive(true);
    }


    public void OpenInfoPanelNarrativeItem(int slotNum) //opens the narrative item panel in the inventory and turns on the text fields and fills them with the info of the current selected weapon.
    {
        infoPanel.SetActive(true);

        useButton.SetActive(false);
        equipButton.SetActive(false);

        //DisableSideButtons();

        effectText.enabled = true;
        effectText.text = "Lucidity:";
        effectAmount.enabled = true;
        description.enabled = true;

        objectiveTitle.enabled = false;
        currentHeaderText.enabled = false;
        completedHeaderText.enabled = false;
        currentObjectiveStepText.enabled = false;
        currentCompletedStepsText.enabled = false;
        objectiveDescription.enabled = false;

        currentNarrativeSlotNum = slotNum;

        weaponIDNum = dataManager.sessionData.narrativeItems[slotNum].id;
        weaponName.text = dataManager.sessionData.narrativeItems[slotNum].name;
        effectAmount.text = dataManager.sessionData.narrativeItems[slotNum].amount.ToString();
        description.text = dataManager.sessionData.narrativeItems[slotNum].description;
    }

    public void OpenInfoPanelWeapon(int slotNum) //opens the ranged weapon panel in the inventory and turns on the text fields and fills them with the info of the current selected weapon.
    {
        infoPanel.SetActive(true);

        useButton.SetActive(false);
        equipButton.SetActive(true);

        //DisableSideButtons();

        effectText.enabled = true;
        effectText.text = "Damage:";
        effectAmount.enabled = true;
        amountText.enabled = true;
        amountText.text = "Ammo:";
        amount.enabled = true;
        description.enabled = true;

        objectiveTitle.enabled = false;
        currentHeaderText.enabled = false;
        completedHeaderText.enabled = false;
        currentObjectiveStepText.enabled = false;
        currentCompletedStepsText.enabled = false;
        objectiveDescription.enabled = false;

        currentSecondarySlotNum = slotNum;

        weaponName.text = dataManager.sessionData.secondaryWeapons[slotNum].name;
        effectAmount.text = weaponDatabase.ReturnItemFromID(slotNum).level1Damage.ToString();
        amount.text = dataManager.sessionData.secondaryWeapons[slotNum].ammo.ToString();
        description.text = dataManager.sessionData.secondaryWeapons[slotNum].description;


        if(dataManager.sessionData.secondaryWeapons[slotNum].id == dataManager.sessionData.secondaryWeapons[dataManager.sessionData.activeSecondaryWeapon].id){
            equippedText.enabled = true;
            Debug.Log("slotNum id: " + dataManager.sessionData.secondaryWeapons[slotNum].id.ToString() + "\nactive weapon id: " + dataManager.sessionData.secondaryWeapons[dataManager.sessionData.activeSecondaryWeapon].id.ToString() + "\n");
        }
        else { equippedText.enabled = false;  }

    }
    public void OpenInfoPanelConsumable(int slotNum) //opens the consumable panel in the inventory and turns on the text fields and fills them with the info of the current selected item.
    {
        infoPanel.SetActive(true);

        useButton.SetActive(true);
        equipButton.SetActive(false);

        //DisableSideButtons();

        effectText.enabled = true;
        effectText.text = "Healing";
        //effectAmount.enabled = true;
        amountText.enabled = true;
        amountText.text = "Amount:";
        amount.enabled = true;
        description.enabled = true;

        objectiveTitle.enabled = false;
        currentHeaderText.enabled = false;
        completedHeaderText.enabled = false;
        currentObjectiveStepText.enabled = false;
        currentCompletedStepsText.enabled = false;
        objectiveDescription.enabled = false;

        currentConsumableSlotNum = slotNum;

        weaponName.text = dataManager.sessionData.consumables[slotNum].itemName;
        amount.text = dataManager.sessionData.consumables[slotNum].amount.ToString();
        //healingamount.text = dataManager.sessionData.consumables[slotNum].amount.ToString();
        description.text = dataManager.sessionData.consumables[slotNum].description;
    }

    public void OpenInfoPanelObjective(int slotNum) //opens the consumable panel in the inventory and turns on the text fields and fills them with the info of the current selected item.
    {
        infoPanel.SetActive(true);

        useButton.SetActive(false);
        equipButton.SetActive(false);

        //DisableSideButtons();

        objectiveTitle.enabled = true;
        currentHeaderText.enabled = true;
        completedHeaderText.enabled = true;
        currentObjectiveStepText.enabled = true;
        currentCompletedStepsText.enabled = true;
        objectiveDescription.enabled = true;

        weaponName.enabled = false;
        effectText.enabled = false;
        effectAmount.enabled = false;
        amountText.enabled = false;
        amount.enabled = false;
        description.enabled = false;
        equippedText.enabled = false;

        currentObjectiveSlotNum = slotNum;

        //todo grab current objective title/description/steps and assign to above text fields in the infopanel
        
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

        //OpenInfoPanelRanged(dataManager.sessionData.secondaryWeapons[dataManager.activeSceondayWeapon].id);

        //CloseInfoPanel();
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

        //EnableSideButtons();

        effectText.enabled = false;
        effectAmount.enabled = false;
        amountText.enabled = false;
        amount.enabled = false;
        description.enabled = false;
    }

    

    private void DisableSideButtons()
    {
        narrativeTab.interactable = false;
        weaponTab.interactable = false;
        consumableTab.interactable = false;
    }

    private void EnableSideButtons()
    {
        narrativeTab.interactable = true;
        weaponTab.interactable = true;
        consumableTab.interactable = true;
    }

}
