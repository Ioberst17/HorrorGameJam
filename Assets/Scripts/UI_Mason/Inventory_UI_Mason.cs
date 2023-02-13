using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_UI_Mason : MonoBehaviour
{

    public DataManager dataManager;

    //dataManager.gameData.inventory;

    [SerializeField] private GameObject inventory;

    [SerializeField] private Image inventoryImage;

    private List<Image> inventorySlots = new List<Image>();



    [SerializeField] private Image slot1;
    [SerializeField] private Image slot2;
    [SerializeField] private Image slot3;
    [SerializeField] private Image slot4;
    [SerializeField] private Image slot5;
    [SerializeField] private Image slot6;
    [SerializeField] private Image slot7;
    [SerializeField] private Image slot8;
    [SerializeField] private Image slot9;
    [SerializeField] private Image slot10;
    [SerializeField] private Image slot11;
    [SerializeField] private Image slot12; 

    public static bool inventoryOpen = false;


    // Start is called before the first frame update
    void Start()
    {
        inventorySlots.Add(slot1);
        inventorySlots.Add(slot2);
        inventorySlots.Add(slot3);
        inventorySlots.Add(slot4);
        inventorySlots.Add(slot5);
        inventorySlots.Add(slot6);
        inventorySlots.Add(slot7);
        inventorySlots.Add(slot8);
        inventorySlots.Add(slot9);
        inventorySlots.Add(slot10);
        inventorySlots.Add(slot11);
        inventorySlots.Add(slot12);




        for (int i = 0; i < dataManager.gameData.secondaryWeapons.Count; i++)
        {
            inventorySlots[i].color = Color.red;
            Debug.Log("weapon=" + dataManager.gameData.secondaryWeapons[i].ID + " /n");
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
            }
            else
            {
                OpenInventory();
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
}
