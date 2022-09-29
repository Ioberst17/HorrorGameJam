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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Instantiate(enemyPrefabs[0], player.transform.position + new Vector3(spawnX, spawnY, spawnZ), Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Instantiate(enemyPrefabs[1], player.transform.position + new Vector3(spawnX, spawnY, spawnZ), Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Instantiate(enemyPrefabs[2], player.transform.position + new Vector3(spawnX, spawnY, spawnZ), Quaternion.identity);
        }
    }
}
