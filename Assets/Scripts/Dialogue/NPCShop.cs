using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class NPCShop<T> : MonoBehaviour where T : IShoppable
{
    private GameObject utilities;
    [SerializeField] private ConsumablesDatabase consumablesDB;
    [SerializeField] private WeaponDatabase weaponsDB;
    private GameObject player;
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private NarrativeItemsManager narrativeItems;
    [SerializeField] private ConsumablesManager consumables;
    [SerializeField] private SecondaryWeaponsManager secondaryWeapons;
    
    // For Checking
    private int currencyIndex;
    private Weapons weaponChecker;
    private Consumables consumablesChecker;
    private bool hasEnoughCurrency;

    [Header("Items for Sale")]
    public List<Weapons> shopWeapons;
    public List<Consumables> shopConsumables; 
    public GameObject shopUI; // Shop UI, TBD on if needed / needs to be built

    private void Start()
    {
        InitializeReferences();
        LoadConsumables();
        LoadWeapons();
    }

    void InitializeReferences()
    {
        utilities = GameObject.Find("Utilities");
        consumablesDB = utilities.GetComponentInChildren<ConsumablesDatabase>();
        weaponsDB = utilities.GetComponentInChildren<WeaponDatabase>();
        player = GameObject.Find("Player");
        inventory = player.GetComponent<InventoryManager>();
        narrativeItems = player.GetComponent<NarrativeItemsManager>();
        consumables = player.GetComponent<ConsumablesManager>();
        secondaryWeapons = player.GetComponent<SecondaryWeaponsManager>();
    }

    void LoadConsumables() { foreach (Consumables entry in consumablesDB.data.entries) { if (entry.isPurchasable) { shopConsumables.Add(entry); } } }

    void LoadWeapons() { foreach (Weapons entry in weaponsDB.data.entries) { if (entry.isPurchasable) { shopWeapons.Add(entry); } } }

    void TryPlayerPurchase(Consumables item)
    {
        //if (/*check if consumable or weapon*/) { }
        UpdateCurrencyIndex();
        hasEnoughCurrency = CheckIfEnoughCurrency(item);
        if (hasEnoughCurrency) { BuyItem(item); }
    }

    // Add an item to the player's inventory and deduct the item's price from their currency
    public void BuyItem(Consumables item)
    {
        // if item is consumable
        inventory.AddConsumable(item.id, item.shopAmountPerPurchase);

        // if item is weapon
        // secondaryWeapons.AddWeapon(/*should pass in the reference*/)
    }

    void UpdateCurrencyIndex() { currencyIndex = consumables.CheckForConsumable("Currency");}

    bool CheckIfEnoughCurrency(Consumables item) // should genericize across consumables / weapons
    {
        if (currencyIndex != -1) 
        {
            if(consumables.consumables[currencyIndex].amount > item.cost)
            {
                consumables.consumables[currencyIndex].amount -= item.cost;
                return true;
            }
            else { Debug.Log("Not enough currency to purchase!"); return false;}
        }
        Debug.Log("Currency is null in player inventory");
        return false;
    }
}
