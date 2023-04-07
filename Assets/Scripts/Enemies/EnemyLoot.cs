using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyLoot : MonoBehaviour
{
    public GameObject droppedItemPrefab;
    [SerializeField] List<Loot> lootList = new List<Loot>();
    private List<Item> ammoList = new List<Item>();
    private int enemyTypeID = -1;
    private EnemyDatabase.Database enemyDB;
    private ConsumablesDatabase.Database consumablesDB;
    private List<GameObject> itemPrefabs;
    float lootLaunchForce = 5f;


    private void Start()
    {
        enemyDB = GameObject.Find("EnemyDatabase").GetComponent<EnemyDatabase>().enemyDatabase;
        consumablesDB = GameObject.Find("ConsumablesDatabase").GetComponent<ConsumablesDatabase>().consumablesDatabase;
        itemPrefabs = GameObject.Find("ItemPrefabs").GetComponent<ItemPrefabs>().itemPrefabs;

        if (CompareTag("Hellhound")) { enemyTypeID = 0; } // add enemyID as in enemy database + behavior component
        else if (CompareTag("Bat")) { enemyTypeID = 1; }
        else if (CompareTag("ParalysisDemon")) { enemyTypeID = 2; }
        else if (CompareTag("Spider")) { enemyTypeID = 3; }
        else if (CompareTag("Bloodgolem")) { enemyTypeID = 4; }
        else if (CompareTag("Gargoyle")) { enemyTypeID = 5; }
        else { enemyTypeID = -1;  Debug.Log("Check for an object named: " + name + "; it is missing an enemy tag"); }

        CreateLootList();
    }

    void CreateLootList()
    {
        if (enemyTypeID != -1)
        {
            for (int i = 0; i < enemyDB.entries.Length; i++)
            {
                if (enemyDB.entries[i].id == enemyTypeID)
                {
                    var entry = enemyDB.entries[i];
                    lootList.Add(new Loot(entry.loot1name, entry.loot1dropChance, entry.loot1amount));
                    lootList.Add(new Loot(entry.loot2name, entry.loot2dropChance, entry.loot2amount));
                    lootList.Add(new Loot(entry.loot3name, entry.loot3dropChance, entry.loot3amount));
                }
            }
        }
    }

    List<Loot> CreateLootDrop()
    {
        List<Loot> possibleItems = new List<Loot>();
        List<Loot> possibleAmmo= new List<Loot>();
        foreach(Loot item in lootList)
        {
            int randomNumber = Random.Range(1, 101);
            if (randomNumber <= item.dropChance) { possibleItems.Add(item); Debug.Log("Item name: " + item.lootName + " was added to the drop"); }
        }
        /*foreach(Loot item in possibleAmmo) // consider implementing ammo drops with enemies by getting current player secondary weapon
        {
            int randomNumber = Random.Range(1, 101);
            if (randomNumber <= item.dropChance) { possibleItems.Add(item); }
            return possibleItems;
        }*/

        //if(possibleItems.Count > 0) { Loot droppedItem = possibleItems[Random.Range(0, possibleItems.Count)]; }

        return possibleItems;
    }

    public void InstantiateLoot(Vector3 spawnPosition)
    {
        List<Loot> DroppedItems = CreateLootDrop();
        if(DroppedItems != null)
        {
         foreach(Loot item in DroppedItems)
            {
                for(int i = 0; i < itemPrefabs.Count; i++)
                {
                    if(item.lootName == itemPrefabs[i].name)
                    {
                        GameObject lootGameObject = Instantiate(itemPrefabs[i], spawnPosition, Quaternion.identity);
                        Debug.Log("Loot was instantiated, it's called: " + lootGameObject.name); // is not being called
                        lootGameObject.GetComponent<Item>().self.amount = item.amount;
                        lootLaunch(lootGameObject);
                    }
                }
            }     
        }
    }

    public void lootLaunch(GameObject lootGameObject)
    {
        Vector2 launchDir = new Vector2(Random.Range(-1f, 1f), Random.Range(0, 1f));
        lootGameObject.GetComponent<Rigidbody2D>().AddForce(launchDir * lootLaunchForce, ForceMode2D.Impulse);
    }
}
