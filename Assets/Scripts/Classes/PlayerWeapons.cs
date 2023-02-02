using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerWeapons
{
    public string weaponName;
    public int weaponID;
    public int weaponLevel;
    public int weaponAmmo;

    public PlayerWeapons(string name, int id, int level, int ammo)
    {
        weaponName = name;
        weaponID = id;
        weaponLevel = level;
        weaponAmmo = ammo;
    }

}
