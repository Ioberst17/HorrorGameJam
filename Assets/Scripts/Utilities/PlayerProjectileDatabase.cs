using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileDatabase : Database<Projectile>
{
    private void Awake()
    {
        numOfColumns = 33;
        textAssetData = Resources.Load<TextAsset>("TextFiles/PlayerProjectileDatabase");
        string[] data = ReadCSV();
        CreateDatabase(data);
    }
}
