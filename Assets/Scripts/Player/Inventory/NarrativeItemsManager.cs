using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class NarrativeItemsManager : MonoBehaviour
{
    public DataManager dataManager;
    public GameObject utilities;
    public NarrativeItemsDatabase narrativeItemsDatabase;
    public List<NarrativeItems> narrativeItems= new List<NarrativeItems>();
    public GameObject player;
    public Lucidity lucidity;
    public float lucidityImpact;

    void Start()
    {
        dataManager = DataManager.Instance;
        utilities = GameObject.Find("Utilities");
        narrativeItemsDatabase = utilities.GetComponentInChildren<NarrativeItemsDatabase>();
        player = GameObject.Find("Player");
        lucidity = player.GetComponentInChildren<Lucidity>();
        Load();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) { AddItem(6); }
    }

    private void Load()
    {
        if (dataManager.sessionData.narrativeItems == null) { narrativeItems = new List<NarrativeItems>(); }
        else { narrativeItems = dataManager.sessionData.narrativeItems; }
        GetCumulativeImpact();
    }

    private void Save() { dataManager.sessionData.narrativeItems = narrativeItems; }

    public void AddItem(int itemID)
    {
        bool foundMatch = false;
        for (int i = 0; i < narrativeItemsDatabase.data.entries.Length; i++)
        {
            if (narrativeItemsDatabase.data.entries[i].id == itemID)
            {
                if(!narrativeItems.Any(obj => obj.id == itemID))
                {
                    var itemToAdd = narrativeItemsDatabase.data.entries[i];
                    narrativeItems.Add(new NarrativeItems(itemToAdd));
                    FindObjectOfType<AudioManager>().PlaySFX(itemToAdd.audioOnAcquisition);
                }
                foundMatch = true;
            }
        }
        if (!foundMatch) { Debug.Log("ERROR: Tried to add a narrative item of value: (" + itemID + ") which does not exist in the database"); }
        GetCumulativeImpact();
    }

    private void GetCumulativeImpact()
    {
        if(narrativeItems.Count == 0) { PassChanges(1); }
        else { foreach (NarrativeItems item in narrativeItems) { if (item.stat == "Lucidity") { lucidityImpact += item.amount; } } }
        PassChanges(lucidityImpact + 1);
    }

    private void PassChanges(float change) { lucidity.narrativeItemRateChange = change; }
}
