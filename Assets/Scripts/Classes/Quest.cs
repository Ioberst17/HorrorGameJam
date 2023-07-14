using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest: IDatabaseItem
{
    // Standard Information
    [SerializeField] private int _id; public int id { get { return _id; } set { _id = value; } }
    [SerializeField] private string _name; public string name { get { return _name; } set { _name = value; } }
    public string description;

    public int numOfSubQuests;

    // information tracked by data manager
    [SerializeField] private int _currentSubQuestIndex; public int currentSubQuestIndex  {  get { return _currentSubQuestIndex; }  set { _currentSubQuestIndex = value; }  }
    [SerializeField] private bool _isStarted; public bool isStarted { get { return _isStarted; } set { _isStarted = value; } }
    [SerializeField] private bool _isActive; public bool isActive { get { return _isActive; } set { _isActive = value; } }
    [SerializeField] private bool _isCompleted; public bool isCompleted { get { return _isCompleted; } set { _isCompleted = value; } }

    public SubQuest[] subQuests;

    public Quest()
    {

    }
    public Quest(int id, string name, string description, int numOfSubQuests)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.numOfSubQuests = numOfSubQuests;
        currentSubQuestIndex = 0;
        isStarted = false;
        isActive = false;
        isCompleted = false;
    }
}

[System.Serializable]
public class SubQuest : IDatabaseItem
{
    [SerializeField] private int _id; public int id { get { return _id; } set { _id = value; } }
    [SerializeField] private string _name; public string name { get { return _name; } set { _name = value; } }
    public enum SubQuestType{ Enter, Talk, Defeat}
    public SubQuestType type;
    public bool needsDedicatedTracking; // if it needs dedicated memory it uses QuestUpdater, else it uses QuestUpdaterSupport;
    public bool isCompleted;

    public SubQuest() { }

    public SubQuest(int id, string task, SubQuestType type)
    {
        this.id = id;
        this.name = task;
        isCompleted = false;
        this.type = type;
    }
}

