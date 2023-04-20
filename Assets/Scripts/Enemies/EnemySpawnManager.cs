using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemySpawnManager : MonoBehaviour
{
    public int areaID;
    public bool enemiesCleared; // used to track if area has been beaten
    public bool respawnEveryTimePlayerEnters;
    public Collider2D spawnTrigger;
    private int index; // for search

    public DataManager dataManager;

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

    public EnemyWave[] waves;
    public int currentWaveIndex { get; private set; } = 0;
    private int numEnemiesSpawned = 0;
    private int numEnemiesDefeated = 0;

    public void Start()
    {
        dataManager = DataManager.Instance;
        spawnTrigger = GetComponent<BoxCollider2D>();
        if(spawnTrigger.isTrigger != true) { spawnTrigger.isTrigger = true; }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null) 
        {
            if (CheckIfAreaCleared()) { }
            else { SpawnEnemies(); }
        }
    }

    void SpawnEnemies()
    {
        if (currentWaveIndex >= waves.Length) { return; }

        EnemyWave currentWave = waves[currentWaveIndex];
        numEnemiesSpawned = currentWave.enemyPrefabs.Length;

        for (int i = 0; i < numEnemiesSpawned; i++)
        {
            GameObject enemy = Instantiate(currentWave.enemyPrefabs[i], currentWave.spawnPoints[i].position, currentWave.spawnPoints[i].rotation);
            enemy.GetComponent<EnemyController>().OnDeath += OnEnemyDefeated;
        }
    }

    void OnEnemyDefeated(EnemyController enemy)
    {
        numEnemiesDefeated++;

        if (numEnemiesDefeated == numEnemiesSpawned)
        {
            EventSystem.current.WaveFinishedTrigger(areaID, currentWaveIndex); Debug.Log("Wave Defeated!");
            currentWaveIndex++;
            numEnemiesDefeated = 0;

            if (currentWaveIndex >= waves.Length)
            {
                EventSystem.current.AllWavesFinishedTrigger(areaID); Debug.Log("All Waves Cleared!");
                if (respawnEveryTimePlayerEnters == false) { enemiesCleared = true; }
                SaveToDataManager();
                return;
            }

            SpawnEnemies();
        }
    }

    private void SaveToDataManager()
    {
        UpdateIndex();

        if(index != -1) { dataManager.gameData.areaHistory.history[index].enemiesCleared = true;}
        else { dataManager.gameData.areaHistory.history.Add(new AreaHistory.History(areaID, true)); }
    }

    private bool CheckIfAreaCleared()
    {
        UpdateIndex();
        if (index != -1)
        {
            if (dataManager.gameData.areaHistory.history[index].enemiesCleared == true) { return true; }
        }
        return false;
    }

    private void UpdateIndex() { index = dataManager.gameData.areaHistory.history.FindIndex(area => area.areaID == areaID); }
}