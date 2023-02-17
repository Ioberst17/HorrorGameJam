using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerWeapons
{
    public string name;
    public int ID;
    public int level;
    public int ammo;
    public bool isSecondary;
    public float fireRate;

    public PlayerWeapons(string name, int id, int level, int ammo, bool isSecondary, float fireRate)
    {
        this.name = name;
        ID = id;
        this.level = level;
        this.ammo = ammo;
        this.isSecondary = isSecondary;
        this.fireRate = fireRate;
    }

}
