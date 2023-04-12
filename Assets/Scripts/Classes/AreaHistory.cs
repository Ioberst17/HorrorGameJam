using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AreaHistory
{
    [System.Serializable]
    public class History
    {
        public int areaID;
        public bool enemiesCleared;

        public History() { }

        public History(int areaID, bool enemiesCleared) { this.areaID = areaID; this.enemiesCleared = enemiesCleared; }
    }

    public List<History> history;

    public AreaHistory() { history = new List<History>(); }
}
