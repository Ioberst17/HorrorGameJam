using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumablesManager : MonoBehaviour
{
    public DataManager dataManager;
    public GameObject utilities;
    public ConsumablesDatabase consumablesDatabase;
    public List<PlayerConsumables> consumables = new List<PlayerConsumables>();


    // Start is called before the first frame update
    void Start()
    {
        dataManager = DataManager.Instance;
        utilities = GameObject.Find("Utilities");
        consumablesDatabase = utilities.GetComponentInChildren<ConsumablesDatabase>();
        Load();
    }

    private void Load()
    {
        if (dataManager.gameData.consumables == null) { consumables = new List<PlayerConsumables>(); }
        else { consumables = dataManager.gameData.consumables; }
    }

    private void Save(){ dataManager.gameData.consumables = consumables; }

    public int CheckForConsumable(int itemID)
    {
        int itemInInv = -1;
        if (consumables != null) { itemInInv = consumables.FindIndex(x => x.id == itemID); }
        return itemInInv;
    }

    public int CheckForConsumable(string itemName)
    {
        int itemInInv = -1;
        if (consumables != null) { itemInInv = consumables.FindIndex(x => x.itemName == itemName); }
        return itemInInv;
    }

    public bool AddExistingItemToInventory(int itemID, int amount)
    {
        bool itemInInv = false;
        if (consumables != null)
        {
            for (int i = 0; i < consumables.Count; i++)
            {
                if (consumables[i].id == itemID)
                {
                    consumables[i].amount += amount;
                    FindObjectOfType<AudioManager>().PlaySFX(consumables[i].audioOnPickup);
                    itemInInv = true;
                }
            }
        }
        return itemInInv;
    }

    public void AddNewItemToInv(int itemID, int amount)
    {
        for (int i = 0; i < consumablesDatabase.data.entries.Length; i++)
        {
            if (consumablesDatabase.data.entries[i].id == itemID)
            {
                var itemToAdd = consumablesDatabase.data.entries[i];

                consumables.Add(new PlayerConsumables(
                    itemToAdd.id,
                    itemToAdd.itemType,
                    itemToAdd.itemName,
                    amount,
                    itemToAdd.audioOnPickup,
                    itemToAdd.audioOnUse,
                    itemToAdd.description));

                FindObjectOfType<AudioManager>().PlaySFX(itemToAdd.audioOnPickup);
            }
        }
    }
}
