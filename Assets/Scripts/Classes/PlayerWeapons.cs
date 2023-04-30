using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerWeapons
{
    public string name;
    public int id;
    public int level;
    public int ammo;
    public bool isSecondary;
    public float fireRate;
    public int ammoLimit;

    public PlayerWeapons(string name, int id, int level, int ammo, bool isSecondary, float fireRate, int ammoLimit)
    {
        this.name = name;
        this.id = id;
        this.level = level;
        this.ammo = ammo;
        this.isSecondary = isSecondary;
        this.fireRate = fireRate;
        this.ammoLimit = ammoLimit;
    }

    public PlayerWeapons(Weapons weapon)
    {
        this.name = weapon.name;
        this.id = weapon.id;
        this.level = weapon.level;
        this.ammo = weapon.ammoLimit;
        this.isSecondary = weapon.isSecondary;
        this.fireRate = weapon.fireRate;
        this.ammoLimit = weapon.ammoLimit;
    }

}
