using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using static ComponentFinder;
using static UnityEngine.GraphicsBuffer;

public class RightArmAnimator : ArmWeaponAnimatorCommonFunctionality
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
        if(OneHandedWeaponInUse) 
        {
            specificFilePathToAnimations = "Animations/Overrides/PlayerBodyParts/RightArm/";
            base.AssignNewAnimations("OneHandedWeapon");
        }
        else if (TwoHandedWeaponInUse)
        {
            specificFilePathToAnimations = "Animations/Overrides/PlayerBodyParts/RightArm/";
            base.AssignNewAnimations("TwoHandedWeapon");
        }

    }

    // Call this method to switch the weapon and update the animator
    public void OnWeaponSwitch(int weaponID, string weaponName, int weaponLevel)
    {
        AssignNewAnimations(weaponName);
    }


}
