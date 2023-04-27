using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class StoryHistory
{
    public Dictionary<string, string> storyStates = new Dictionary<string, string>();

    public void AddStoryState(string storyID, string state)
    {
        if (!storyStates.ContainsKey(storyID))
        {
            storyStates.Add(storyID, state);
        }
        else
        {
            storyStates[storyID] = state;
        }
    }

    public string GetStoryState(string storyID)
    {
        if (storyStates.ContainsKey(storyID))
        {
            return storyStates[storyID];
        }
        else
        {
            return null;
        }
    }

    public void Clear()
    {
        storyStates.Clear();
    }
}
