using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
//using static UnityEditor.Progress;

[RequireComponent(typeof(BreakablesData))]
public class BreakableLoot : MonoBehaviour
{
    [SerializeField] InventoryManager inventory;
    [SerializeField] PlayerController playerController;
    [SerializeField] ConsumablesDatabase.Database consumablesDB;
    private List<GameObject> itemPrefabs;
    float lootLaunchForce = 5f;

    private void Start()
    {
        inventory = GameObject.Find("Player").GetComponent<InventoryManager>();
        playerController = GameObject.Find("PlayerModel").GetComponent<PlayerController>();
        consumablesDB = GameObject.Find("ConsumablesDatabase").GetComponent<ConsumablesDatabase>().consumablesDatabase;
        itemPrefabs = GameObject.Find("ItemPrefabs").GetComponent<ItemPrefabs>().itemPrefabs;
    }

    public void GenerateLoot()
    {
        GenerateHearts();
        GenerateAmmo();
    }

    public void GenerateHearts() 
    {
        string assetPath = "ItemPrefabs/Heart";
        int numOfHearts = BreakablesData.breakablesData["Crate"].heartsAmount;
        for (int j = 0; j < numOfHearts; j++)
        {
            GameObject lootGameObject = Instantiate(Resources.Load(assetPath) as GameObject, transform.position, Quaternion.identity);
            lootLaunch(lootGameObject);
        }

        /*GameObject toInstantiate = Resources.Load("ItemPrefabs/Heart") as GameObject;
        Instantiate(toInstantiate, transform.parent);
        Debug.Log("Instantiated game object: " + toInstantiate.name);
        lootLaunch(toInstantiate);*/
    }

    public void GenerateAmmo() 
    {
        string currentSecondary = inventory.secondaryWeaponsManager.GetCurrentWeaponName();
        string assetPath = "ItemPrefabs/Ammo/" + currentSecondary;
        GameObject toInstantiate = Instantiate(Resources.Load(assetPath) as GameObject, transform.position, Quaternion.identity);
        Debug.Log("Instantiated game object: " + toInstantiate.name);
        Instantiate(toInstantiate, transform.parent);
        lootLaunch(toInstantiate);
    }


    public void lootLaunch(GameObject lootGameObject)
    {
        Vector2 launchDir = new Vector2(Random.Range(-1f, 1f), Random.Range(0, 1f));
        lootGameObject.GetComponent<Rigidbody2D>().AddForce(launchDir * lootLaunchForce, ForceMode2D.Impulse);
    }
}
