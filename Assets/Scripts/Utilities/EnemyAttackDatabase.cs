using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackDatabase : Database<Attack>
{
    private void Awake()
    {
        numOfColumns = 23;
        textAssetData = Resources.Load<TextAsset>("TextFiles/EnemyAttackDatabase");
        string[] data = ReadCSV();
        CreateDatabase(data);
    }
}
