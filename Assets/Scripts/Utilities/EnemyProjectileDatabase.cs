using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileDatabase : Database<Projectile>
{
    private void Awake()
    {
        numOfColumns = 33;
        textAssetData = Resources.Load<TextAsset>("TextFiles/EnemyProjectileDatabase");
        string[] data = ReadCSV();
        CreateDatabase(data);
    }
}
