using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mixpanel;
using System;
using UnityEngine.SceneManagement;
using System.Linq;
using Ink.Parsed;
using System.Reflection;

public class EnemyCreationForTesting : MonoBehaviour
{
    /* DECLARATIONS */

    public GameObject[] enemyPrefabs;
    public Transform[] areaSpawnPoints;
    [SerializeField] private SpawnPoint[] levelSpawnPoints;

    // enemy spawnings
    public GameObject spawnedEnemy;
    public Collider2D enemyCollider;

    // used for as data for positioning
    public GameObject player;
    public PlayerController playerController;
    public Vector3 playerPosition;

    // used in positioning calculations
    public float[] possibleSpawnPointDistances;
    public Vector3 chosenSpawnPoint;
    public float maxDistance;
    public float distanceToCompare;
    public int spawnX;
    public int spawnY;
    public int spawnZ;

    //positioning logic
    private Func<float[], int> midpointIndex = arr => (arr.Length - 1) / 2; // returns the index value of the midpoint value in array

    [System.Serializable]
    public struct SpawnPoint
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    [System.Serializable]
    public struct EnemyWave
    {
        public GameObject[] enemyPrefabs;
        public SpawnPoint[] spawnPoints;
    }

    private void Start() 
    { 
        player = GameObject.FindGameObjectWithTag("Player"); 
        playerController = player.GetComponentInChildren<PlayerController>();
        TurnTransformsIntoSpawnPoints();
    }

    // Needed since SpawnPoint struct can be serialized i.e. saved; Transforms cannot, but are easier to place)
    void TurnTransformsIntoSpawnPoints()
    {
        // Create a new List of SpawnPoints to hold the converted Transform data
        List<SpawnPoint> newSpawnPoints = new List<SpawnPoint>();

        // Loop through each array of Transforms in areaSpawnPoints
        foreach (Transform transform in areaSpawnPoints)
        {
            SpawnPoint spawnPoint = new SpawnPoint();
            spawnPoint.position = transform.position;
            spawnPoint.rotation = transform.rotation;

            // Add the new SpawnPoint to the list
            newSpawnPoints.Add(spawnPoint);
        }

        // Convert the list to an array and assign it to levelSpawnPoints
        levelSpawnPoints = newSpawnPoints.ToArray();
        possibleSpawnPointDistances = new float[levelSpawnPoints.Length];
}


    public void SpawnEnemy(int idNum) // main function
    {
        if (idNum >= 0 && idNum <=7)
        {
            if (SceneManager.GetActiveScene().name == "CombatMode") // in combat mode spawning, from spawnPoints
            {
                chosenSpawnPoint = PickSpawnPoint();
                spawnedEnemy = Instantiate(enemyPrefabs[idNum], chosenSpawnPoint, Quaternion.identity);
                enemyCollider = spawnedEnemy.GetComponent<Collider2D>();
                spawnedEnemy.transform.position = AdjustIfSpawnHasCollision(spawnedEnemy.transform.position, enemyCollider);
                Debug.Log("Spawned " + enemyPrefabs[idNum].name + " at location; " + spawnedEnemy.transform.position + ". Its position before any adjustment was: " + chosenSpawnPoint);  ;
            }
            else // in standard game debug spawning, relative to player spawnpoint
            {
                var spawnLocation = player.transform.position + new Vector3(spawnX, spawnY, spawnZ);
                spawnedEnemy = Instantiate(enemyPrefabs[idNum], spawnLocation, Quaternion.identity);
                Debug.Log("Spawned " + enemyPrefabs[idNum].name + " at location " + chosenSpawnPoint);
            }  
            TrackDataInMixPanel(idNum);
        }
    }

    /// <summary>
    /// // Get all distances and choose a spawnPoint based on the public array levelSpawnPoints
    /// </summary>
    /// <returns>Returns Vector3 world position information for pawn</returns>
    Vector3 PickSpawnPoint() 
    {
        return ChooseASpawnPoint(GetSpawnPointDistances());
    }

    float[] GetSpawnPointDistances()
    {
        Array.Clear(possibleSpawnPointDistances, 0, possibleSpawnPointDistances.Length);
        playerPosition = playerController.transform.position;

        for (int i = 0; i < levelSpawnPoints.Length; i++) 
        {
            possibleSpawnPointDistances[i] = Vector3.Distance(levelSpawnPoints[i].position, playerPosition);
        }
        return possibleSpawnPointDistances;
    }

    Vector3 ChooseASpawnPoint(float[] possibleSpawnPointDistances)
    {
        return levelSpawnPoints[SelectionLogic(possibleSpawnPointDistances)].position;
    }

    /// <summary>
    /// Adjusts the spawn position of an enemy upwards to avoid ground collisions.
    /// </summary>
    /// <param name="idNum">Index of the enemy prefab in the array.</param>
    /// <param name="spawnLocation">Original spawn location.</param>
    /// <returns>New spawn position.</returns>
    private Vector3 AdjustIfSpawnHasCollision(Vector3 currentPosition, Collider2D enemyCollider)
    {
        // OverlapBoxAll requires half extents, not bounds size directly
        Vector2 halfExtents = enemyCollider.bounds.extents * 0.5f;

        // Gets all ground/environment colliders that overlap with the enemy collider
        Collider2D[] colliders = Physics2D.OverlapBoxAll(currentPosition + (Vector3)enemyCollider.offset, halfExtents, 0f, LayerMask.GetMask("Environment"));

        // Adjust enemy position up until it doesn't overlap with any environment colliders
        foreach (Collider2D collider in colliders)
        {
            if (collider == null) continue;

            // Check if the collider is on the "Environment" layer
            if (collider.gameObject.layer == LayerMask.NameToLayer("Environment"))
            {
                // Adjust the spawn location and recheck
                while (enemyCollider.bounds.Intersects(collider.bounds))
                {
                    currentPosition += Vector3.up * 0.5f;
                }
            }
        }

        // Adjust the spawn location to be above ground (in case overlapBoxAll missed something)
        RaycastHit2D hit = Physics2D.Raycast(currentPosition, Vector2.down, 100f, LayerMask.GetMask("Environment"));
        if (hit.collider != null)
        {
            currentPosition = hit.point + Vector2.up * 0.5f;
        }

        return currentPosition;
    }


    void TrackDataInMixPanel(int idNum)
    {
        // for data tracking to mixpanel
        var props = new Value();
        props["Enemy Name"] = enemyPrefabs[idNum].name;
        Mixpanel.Track("Enemies Generated", props);

        Mixpanel.Flush();
    }

    // Algorithms shenanigans below related to picking a spawnPoint i.e. picking a median from an unsorted array:
    public static int SelectionLogic(float[] arr)
    {
        int middleVal = arr.Length / 2;

        if(arr.Length == 0){ Debug.Log("Check if spawn points are properly initialized"); return -1; }
        else if (arr.Length == 1) { return 0; }
        else if (arr.Length == 2) { return Array.FindIndex(arr, x => x == arr.Min()); }

        /*Longer cases use a lambda to compare each element of the array to find the median val, then return an index
         * 1. Use Count to get num of elements (i) less than current element x + (ii) number of elements > or = to x
         * 2  Median is the value that has 'middleVal' elements (i) less than it & (ii) (numbers.Length % 2 == 0) ? 2 : 1 elements that are = or >. 
         * 3. If arr.Length is even, then the median is the avg. of 2 middle values, so it will have two occurrences in the array.
         * 4. So if even, it returns the index of the  of the 2 middleVals, if odd  it returns the 1 index of the median */
        
        else
        { return Array.FindIndex(arr, x => arr.Count(y => y < x) == middleVal && arr.Count(y => y == x) == ((arr.Length % 2 == 0) ? 2 : 1)); }
    }

}
