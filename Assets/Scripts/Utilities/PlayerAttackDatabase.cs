using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackDatabase : Database<Attack>
{
    private void Awake()
    {
        numOfColumns = 23;
        textAssetData = Resources.Load<TextAsset>("TextFiles/PlayerAttackDatabase");
        string[] data = ReadCSV();
        CreateDatabase(data);
    }
}
