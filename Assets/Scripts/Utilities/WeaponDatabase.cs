using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using Ink.Parsed;
using Ink.Runtime;
using JetBrains.Annotations;
using System.Linq;
using Unity.VisualScripting;

public class WeaponDatabase : Database<Weapons>
{
    // must be attached to a game object in the scene hierarchy
    // creates a weapons database (the store of weapons and information about them), and reads it from the weaponDatbase.csv in /Resources

    private Weapons weaponChecker;

    public List<int> validPrimaryWeaponIDs;
    public List<int> validSecondaryWeaponIDs;

    private void Awake()
    {
        numOfColumns = 27;
        textAssetData = Resources.Load<TextAsset>("TextFiles/WeaponDatabase");
        string[] data = ReadCSV();
        CreateDatabase(data);

        validPrimaryWeaponIDs = CreateListOfValidWeapons("Primary");
        validSecondaryWeaponIDs = CreateListOfValidWeapons("Secondary");
    }


    private List<int> CreateListOfValidWeapons(string PrimaryOrSecondary)
    {
        List<int> checker = new List<int>();

        for (int i = 0; i < data.entries.Length; i++)
        {
            if (PrimaryOrSecondary == "Secondary")
            {
                if (data.entries[i].isSecondary) { checker.Add(data.entries[i].id); }
            }
            else if (PrimaryOrSecondary == "Primary")
            {
                if (!data.entries[i].isSecondary) { checker.Add(data.entries[i].id); }
            }
        }

        return checker;
    }

    public int GetWeaponDamage(int weaponID, int LevelOfWeapon)
    {
        var ammoLevel = LevelOfWeapon - 1;

        if (ammoLevel == 0) { return data.entries[weaponID].level1Damage; }
        else if (ammoLevel == 1) { return data.entries[weaponID].level2Damage; }
        else if (ammoLevel == 2) { return data.entries[weaponID].level3Damage; }
        else { Debug.LogFormat("Check to see if the correct weaponID and a Level of Weapon are being inputted to this function"); return -1; }
    }

    public string GetWeaponEffect(int weaponID)
    {
        weaponChecker = Array.Find(data.entries, x => x.id == weaponID);
        if (weaponChecker != null) { return weaponChecker.statusModifier; }

        Debug.Log("Weapon ID could not be found in weapon database; please check input");
        return "Error";
    }
}