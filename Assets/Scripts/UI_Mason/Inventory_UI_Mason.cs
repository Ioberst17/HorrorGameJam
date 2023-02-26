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

    //dataManager.gameData.inventory;

    [SerializeField] private GameObject inventory;

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
        //inventorySlots.Add(slot11);
        //inventorySlots.Add(slot12);

        consumableSlots.Add(consumables_Slot1);
        consumableSlots.Add(consumables_Slot2);

        rangedAmmoNumbers.Add(r_Ammo1);
        rangedAmmoNumbers.Add(r_Ammo2);
        rangedAmmoNumbers.Add(r_Ammo3);
        rangedAmmoNumbers.Add(r_Ammo4);
        rangedAmmoNumbers.Add(r_Ammo5);
        rangedAmmoNumbers.Add(r_Ammo6);


        for (int i = 0; i < meleeSlots.Count; i++)
        {
            meleeSlots[i].SetActive(false);

            Debug.Log("melee slots hidden.\n");
        }

        for (int i = 0; i < rangedSlots.Count; i++)
        {
            rangedSlots[i].SetActive(false);
            Debug.Log("ranged slots hidden.\n");
        }

        for (int i = 0; i < consumableSlots.Count; i++)
        {
            consumableSlots[i].SetActive(false);
            Debug.Log("consumable slots hidden.\n");
        }

        Debug.Log("Melee Weapon Count: " + dataManager.gameData.primaryWeapons.Count);
        Debug.Log("Ranged Weapon Count: " + dataManager.gameData.secondaryWeapons.Count);
        Debug.Log("Consumable Count: " + dataManager.gameData.consumables.Count);

        Debug.Log("ammo for ranged slot 1: " + dataManager.gameData.secondaryWeapons[1].ammo);

        //Debug.Log("secondary weapon 1 toString: " + dataManager.gameData.secondaryWeapons[1].ammo.ToString());

       // ammo = dataManager.gameData.secondaryWeapons[1].ammo;

        //Debug.Log("ammo for ranged weapon 1: " + ammo);

        //Debug.Log("ranged ammo number count: " + rangedAmmoNumbers[1].text);

       // rangedAmmoNumbers[1].text = dataManager.gameData.secondaryWeapons[1].ammo.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inventoryOpen)
            {
                CloseInventory();
                Time.timeScale = 1.0f;
            }
            else
            {
                OpenInventory();
                Time.timeScale = 0f;
            }

            for (int i = 0; i < dataManager.gameData.primaryWeapons.Count; i++)
            {
                meleeSlots[i].SetActive(true);
                Debug.Log("weapon=" + dataManager.gameData.primaryWeapons[i].id + " /n");
            }

            for (int i = 0; i < dataManager.gameData.secondaryWeapons.Count; i++)
            {
                
                rangedSlots[i].SetActive(true);
                rangedAmmoNumbers[i].text = dataManager.gameData.secondaryWeapons[i].ammo.ToString();
                Debug.Log(" rangedweapon=" + dataManager.gameData.secondaryWeapons[i].id + " /n");
            }

            for (int i = 0; i < dataManager.gameData.consumables.Count; i++)
            {
                consumableSlots[i].SetActive(true);
                Debug.Log("consumable=" + dataManager.gameData.consumables[i].id + " /n");
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

    public void OpenMeleeInventory()
    {
        melee_Inventory.SetActive(true);
        ranged_Inventory.SetActive(false);
        consumables_Inventory.SetActive(false);
    }

    public void OpenRangedInventory()
    {
        melee_Inventory.SetActive(false);
        ranged_Inventory.SetActive(true);
        consumables_Inventory.SetActive(false);
    }

    public void OpenConsumablesInventory()
    {
        melee_Inventory.SetActive(false);
        ranged_Inventory.SetActive(false);
        consumables_Inventory.SetActive(true);
    }

}
