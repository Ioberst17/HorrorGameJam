using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryWeaponsManager : WeaponsManager
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void WeaponUIUpdate()
    {
        if (weaponList.Count > 0)
        {
            string weaponName = weaponList[currentWeaponIndex].name;
            int weaponAmmo = weaponList[currentWeaponIndex].ammo;

            int weaponID = weaponList[currentWeaponIndex].id;
            int weaponLevel = weaponList[currentWeaponIndex].level;

            EventSystem.current.UpdatePrimaryWeaponUITrigger(weaponName, weaponAmmo);
            EventSystem.current.UpdatePrimaryWeaponTrigger(weaponID, weaponLevel);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

}
