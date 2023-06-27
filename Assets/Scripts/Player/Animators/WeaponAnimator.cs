using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class WeaponAnimator : ArmWeaponAnimatorCommonFunctionality
{
    Quaternion animationRotation;

    // for vfx animations
    [SerializeField] Transform currentFirePoint;
    FirePointAnimator firePointAnimator;
    int animationToGenerate;
    string pathOfAnimationToGenerate;

    // event information to cache before passing along
    int weaponID;
    int weaponLevel; 
    int ammoChange;
    int currentAmmoLevel;


    override public void Start()
    {
        base.Start();

        // get key references
        firePointAnimator = GetComponentInChildren<FirePointAnimator>();
        currentFirePoint = firePointAnimator.transform;

        EventSystem.current.onUpdateSecondaryWeaponTrigger += OnWeaponSwitch;
        EventSystem.current.onPlayerShotInformation += CacheShotInfo;
    }

    private void OnDestroy()
    {
        EventSystem.current.onUpdateSecondaryWeaponTrigger -= OnWeaponSwitch;
        EventSystem.current.onPlayerShotInformation -= CacheShotInfo;
    }

    override public void AssignNewAnimations(string weaponName)
    {
        specificFilePathToAnimations = "Animations/Overrides/PlayerWeapons/";
        base.AssignNewAnimations(weaponName);
    }

    // Call this method to switch the weapon and update the animator
    public void OnWeaponSwitch(int weaponID, string weaponName, int weaponLevel)
    {
        AssignNewAnimations(weaponName);
    }

    // generate a random shot vfx
    private void GenerateShotVFX()
    {
        // get path to random shot vfx
        animationToGenerate = UnityEngine.Random.Range(1, 10);
        pathOfAnimationToGenerate = "VFXPrefabs/GunFireVFX/ef_" + animationToGenerate.ToString();
        // rotate based on facing direction and then load
        if (startedShotOnLeft) { animationRotation = Quaternion.Euler(MapLeftFacingShotVFX(shotDirectionRotation.eulerAngles.z), -90, 90); }
        else { animationRotation = Quaternion.Euler(shotDirectionRotation.eulerAngles.z - 90, -90, 90); }
        // Generate
        Instantiate(Resources.Load<GameObject>(pathOfAnimationToGenerate), currentFirePoint.position, animationRotation);
    }

    // handles rotation of the shot VFX to make it face the right direction
    // Temp: should inspect actual odd physics / rotation to obviate need for
    float MapLeftFacingShotVFX(float value)
    {
        if (value >= 0 && value <= 90) { return 90 - value; }
        else if (value >= 270 && value <= 360)
        {
            float normalizedValue = (value - 270) / (360 - 270); // normalize input
            return normalizedValue * (90 - 180) + 180; // to get to an output b/w 90 and 180
        } 
        else
        {
            Debug.Log("Value out of range!");
            return value;
        }
    }

    // Adds shot vfx at end of parent's function which handles pointing of weapon
    override public void SetPointingDirection(bool optionalForceLeftFacingDirection = false)
    {
        base.SetPointingDirection(optionalForceLeftFacingDirection);
        if (currentWeapon.isShot) { GenerateShotVFX(); }
        // sent to playerSecondaryWeapon
        EventSystem.current.ReleaseAmmo(
                            weaponID,
                            weaponLevel,
                            ammoChange,
                            currentAmmoLevel); 
    }

    // stores info when releasing ammo when pointing weapon
    void CacheShotInfo(int weaponID, int weaponLevel, int ammoChange, int currentAmmoLevel)
    {
        this.weaponID = weaponID;
        this.weaponLevel = weaponLevel;
        this.ammoChange = ammoChange;
        this.currentAmmoLevel = currentAmmoLevel;
    }
}

