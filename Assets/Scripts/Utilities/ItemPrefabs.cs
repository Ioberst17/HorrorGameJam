using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class ItemPrefabs : MonoBehaviour
{
    public List<GameObject> itemPrefabs;
    private ConsumablesDatabase.DB dataBase;

    // Start is called before the first frame update
    void Start()
    {
        itemPrefabs = Resources.LoadAll<GameObject>("ItemPrefabs").ToList();
        dataBase = GameObject.Find("ConsumablesDatabase").GetComponent<ConsumablesDatabase>().data;

        UpdatePrefabs();
    }

    void UpdatePrefabs()
    {

        foreach(GameObject item in itemPrefabs)
        {
            for (int i = 0; i < dataBase.entries.Length; i++)
            {
                if (item.GetComponent<Item>().staticID == dataBase.entries[i].id)
                {
                    item.GetComponent<Item>().self = new Consumables (dataBase.entries[i]);
                } 
            }
        }
    }

}
