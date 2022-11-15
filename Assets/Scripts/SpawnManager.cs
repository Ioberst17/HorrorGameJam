using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    /* DECLARATIONS */

    public GameObject[] enemyPrefabs;
    public GameObject player;
    public int spawnX;
    public int spawnY;
    public int spawnZ;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnEnemy(int idNum)
    {
        if (idNum >= 0 && idNum <=6)
        {
            var spawnLocation = player.transform.position + new Vector3(spawnX, spawnY, spawnZ);
            Instantiate(enemyPrefabs[idNum], spawnLocation, Quaternion.identity);
            Debug.Log("Spawned " + enemyPrefabs[idNum].name + " at location " + spawnLocation);
        }
    }
}
