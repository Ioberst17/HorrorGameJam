using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FindObjectsByFileID : MonoBehaviour
{
    [SerializeField]
    List<int> fileIDs;
    Object tempObj;

    void Start()
    {
        if (fileIDs != null)
        {
            // get the object with the specified fileID
            foreach (int fileID in fileIDs)
            {
                tempObj = EditorUtility.InstanceIDToObject(fileID);
                if (tempObj != null)
                {
                    // if the object exists, print its name
                    Debug.Log("Object of the name was found: " + tempObj.name + "; it's fileID is: " + fileID);
                }
                else
                {
                    // if the object does not exist, print an error message
                    Debug.LogError("Object with fileID " + fileID + " not found.");
                }
            }
        }
    }
}
