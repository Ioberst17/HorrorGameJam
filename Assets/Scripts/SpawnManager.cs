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
        if (idNum == 0)
        {
            Instantiate(enemyPrefabs[0], player.transform.position + new Vector3(spawnX, spawnY, spawnZ), Quaternion.identity);
        }

        if (idNum == 1)
        {
            Instantiate(enemyPrefabs[1], player.transform.position + new Vector3(spawnX, spawnY, spawnZ), Quaternion.identity);
        }

        if (idNum == 2)
        {
            Instantiate(enemyPrefabs[2], player.transform.position + new Vector3(spawnX, spawnY, spawnZ), Quaternion.identity);
        }
        if (idNum == 3)
        {
            Instantiate(enemyPrefabs[3], player.transform.position + new Vector3(spawnX, spawnY, spawnZ), Quaternion.identity);
        }
        if (idNum == 4)
        {
            Instantiate(enemyPrefabs[4], player.transform.position + new Vector3(spawnX, spawnY, spawnZ), Quaternion.identity);
        }
        if (idNum == 5)
        {
            Instantiate(enemyPrefabs[5], player.transform.position + new Vector3(spawnX, spawnY, spawnZ), Quaternion.identity);
        }
        if (idNum == 6)
        {
            Instantiate(enemyPrefabs[6], player.transform.position + new Vector3(spawnX, spawnY, spawnZ), Quaternion.identity);
        }
    }
}
