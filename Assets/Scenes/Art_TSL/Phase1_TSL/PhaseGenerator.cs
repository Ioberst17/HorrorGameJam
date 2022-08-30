// The purpose of this script is to generate a phase made of randomly pooled levels that are connected together that scales with difficulty.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseGenerator : MonoBehaviour
{
    //// Since we only need to generate a phase once per phase, this will be called in another script when the game begins.

    //// Initialize all variables to default 0 or empty values.
    //public object[] roomsArray = null; // no room objects to start
    //public int maxRoomCount = 0; // default 0 value
    //public int difficultyLevel = 0; // default level is 0 for no added difficulty
    //public int phaseLevel = 1; // always starts at phase1

    //// This class will need a function that grabs a room from a folder of premade room assets, determines the possible directions
    //// that a room can be placed, and places that room, then iterates until the phase has met the limit advised by the difficulty level (int) and phase number (int).
    //// NOTE: increasing level = increasing difficulty and complexity
    //// Parameters: needs a difficulty level (normal=0,hard=1), needs a phase number.
    //public static void GeneratePhase(int difficultyLevel, int phaseLevel)
    //{
    //    // references the Room Asset Folder and stores all rooms in an Object array.
    //    roomsArray = AssetDatabase.LoadAllAssetsAtPath("Assets/Scenes/" + "Phase" + phaseLevel + "_TSL/Phase" + phaseLevel + "Rooms_TSL");
    //    // max room count should be determined by the difficulty rating and phase number.
    //    // max room count formula:
        
    //    // PhaseLevel: 1 = 6 rooms: 1 boss room, (phaseNumber + difficultyRating)*2 enemy rooms, phaseNumber of non-enemy rooms (hasTreasure or !hasTreasure)
    //    //             2 = increases...
    //    maxRoomCount = phaseLevel * 2 + 4;
    //    //enemyRoomCount = phaseLevel; //WORK IN-PROGRESS

    //    // randomly select a room from the roomsArray and select it as the starting room.
    //    // Then, select another room with a door that connects to an appropriate door opening.
    //    // Then, repeat until the level has the appropriate number of max room count and has a boss exit room.
        
    //}

}
