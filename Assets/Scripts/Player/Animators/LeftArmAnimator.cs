using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftArmAnimator : ArmWeaponAnimatorCommonFunctionality
{
    override public void Start()
    {
        base.Start();
        EventSystem.current.onUpdateSecondaryWeaponTrigger += OnWeaponSwitch;
    }
    private void OnDestroy()
    {
        EventSystem.current.onUpdateSecondaryWeaponTrigger -= OnWeaponSwitch;
    }

    override public void AssignNewAnimations(string weaponName)
    {
        if (oneHandedWeaponInUse) // may not work since child funciton is called first
        {
            specificFilePathToAnimations = "Animations/Overrides/PlayerBodyParts/LeftArm/";
            base.AssignNewAnimations("OneHandedWeapon");
        }
        else if (twoHandedWeaponInUse)
        {
            specificFilePathToAnimations = "Animations/Overrides/PlayerBodyParts/LeftArm/";
            base.AssignNewAnimations("TwoHandedWeapon");
        }

    }

    // Call this method to switch the weapon and update the animator
    public void OnWeaponSwitch(int weaponID, string weaponName, int weaponLevel)
    {
        AssignNewAnimations(weaponName);
    }
}
