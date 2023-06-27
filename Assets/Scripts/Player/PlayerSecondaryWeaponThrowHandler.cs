using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSecondaryWeaponThrowHandler : MonoBehaviour
{
    //External References
    GameController gameController;
    SecondaryWeaponsManager secondaryWeaponsManager;
    PlayerAnimator animator;

    //Needed from PlayerSecondaryWeapon
    PlayerSecondaryWeapon playerSecondaryWeapon;
    Transform projectileSpawnPoint;

    [Header("Throwing Variables")]
    [SerializeField] private float maxForceHoldDownTime = 2f;
    [SerializeField] private float maxThrowBand = 2f;
    [SerializeField] private static int maxThrowSpeed = 10;
    [SerializeField] private float throwMultiplier;
    private static int minThrowSpeed = 5;
    public int throwSpeed;
    private int throwGravity = 1;
    private float throwKeyHeldDownStart;
    private float throwKeyReleased;
    [SerializeField] private float throwKeyHeldDownTime;
    [SerializeField] private float holdTimeNormalized;
    [SerializeField] private float throwDistanceNormalized;
    public bool inActiveThrow = false;
    private float currentThrowForce = 0;
    private int currentThrowDirection = 0;

    [SerializeField] private Vector3 throwMouseStartingPos;
    [SerializeField] private Vector3 throwMouseFinishingPos;
    private float throwDistanceToPass;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
        secondaryWeaponsManager = GetComponentInParent<SecondaryWeaponsManager>();
        animator = ComponentFinder.GetComponentInChildrenByNameAndType<PlayerAnimator>("Animator", transform.parent.gameObject);
        playerSecondaryWeapon = GetComponent<PlayerSecondaryWeapon>();
        projectileSpawnPoint = playerSecondaryWeapon.projectileSpawnPoint;
    }

    // tracks data around clicks and time held
    public void HandleThrowing(string inputState, string currentControlScheme)
    {
        if (inputState == "Button Clicked")
        {
            if (playerSecondaryWeapon.currentWeapon.isThrown)
            {
                inActiveThrow = true;
                secondaryWeaponsManager.ChangingIsBlocked = true;
                if (currentControlScheme == "Keyboard and Mouse") { throwMouseStartingPos = gameController.playerPosition; }
                if (currentControlScheme == "Gamepad") { throwMouseStartingPos = gameController.playerPositionScreen; } //throwMouseStartingPos = Input.mousePosition; //throwKeyHeldDownStart = Time.time;
            } 
        }
        else if (inputState == "Button Released")
        {
            throwKeyReleased = Time.time;
            if (inActiveThrow) 
            {
                animator.Play("PlayerThrow");
                ThrowWeapon(currentThrowForce); 
            }
        }
        if (inputState == "Button Held")
        {
            if (inActiveThrow)
            {
                if (currentControlScheme == "Keyboard and Mouse") { throwMouseFinishingPos = gameController.lookInputWorld; }
                throwDistanceToPass = Vector2.Distance(throwMouseFinishingPos, throwMouseStartingPos) * throwMultiplier;
                currentThrowForce = CalcThrowForce(throwDistanceToPass);
            }
        }
    }

    private float CalcThrowForce(float holdForce)
    {
        throwDistanceNormalized = Mathf.Clamp01(holdForce / maxThrowBand);
        float force = throwDistanceNormalized * maxThrowSpeed;
        force += minThrowSpeed;
        EventSystem.current.StartChargedAttackTrigger(throwDistanceNormalized, projectileSpawnPoint.transform, force);
        return force;
    }

    public void ThrowWeapon(float throwSpeed)
    {
        GameObject toss = Instantiate(playerSecondaryWeapon.ammoPrefabs[playerSecondaryWeapon.currentAmmoIndex], projectileSpawnPoint.position, projectileSpawnPoint.transform.rotation);
        Vector3 bulletDir = ((Vector3)gameController.lookInput - (Vector3)gameController.playerPositionScreen).normalized;
        FindObjectOfType<AudioManager>().PlaySFX("WeaponToss");

        toss.GetComponent<Rigidbody2D>().gravityScale = throwGravity;

        if (10 > transform.rotation.eulerAngles.z && transform.rotation.eulerAngles.z > -10)
        {
            toss.GetComponent<Rigidbody2D>().AddForce(bulletDir * throwSpeed, ForceMode2D.Impulse);
        }
        else { toss.GetComponent<Rigidbody2D>().AddForce( bulletDir * throwSpeed, ForceMode2D.Impulse); }

        inActiveThrow = false;
        secondaryWeaponsManager.ChangingIsBlocked = false;
        EventSystem.current.FinishChargedAttackTrigger();
    }
}
