using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerWeapons
{
    public int weaponID;
    public int weaponLevel;
    public int weaponAmmo;

    public PlayerWeapons(int id, int level, int ammo)
    {
        weaponID = id;
        weaponLevel = level;
        weaponAmmo = ammo;
    }

}
