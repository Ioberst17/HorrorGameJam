using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null) { SpawnEnemies(); }
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
            EventSystem.current.WaveFinishedTrigger(currentWaveIndex); Debug.Log("Wave Defeated!");
            currentWaveIndex++;
            numEnemiesDefeated = 0;

            if (currentWaveIndex >= waves.Length)
            {
                EventSystem.current.AllWavesFinishedTrigger(); Debug.Log("All Waves Cleared!");
                return;
            }

            SpawnEnemies();
        }
    }

}