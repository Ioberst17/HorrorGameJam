using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrowHandler : MonoBehaviour
{
    //External References
    GameController gameController;
    SecondaryWeaponsManager secondaryWeaponsManager;
    PlayerAnimator animator;

    //Needed from PlayerSecondaryWeapon
    PlayerSecondaryWeapon playerSecondaryWeapon;
    [SerializeField] Transform projectileSpawnPoint;

    [Header("Throwing Variables")]
    [SerializeField] float maxForceHoldDownTime = 2f;
    [SerializeField] float maxThrowBand = 2f;
    [SerializeField] static int maxThrowSpeed = 10;
    [SerializeField] float throwMultiplier;
    static int minThrowSpeed = 5;
    public int throwSpeed;
    float throwKeyReleased;
    [SerializeField] private float throwKeyHeldDownTime;
    [SerializeField] private float holdTimeNormalized;
    [SerializeField] private float throwDistanceNormalized;
    [SerializeField] private bool _inActiveThrow; public bool InActiveThrow { get { return _inActiveThrow; } set { _inActiveThrow = value; } }
    [SerializeField] private float _currentThrowForce; public float CurrentThrowForce { get { return _currentThrowForce; } set { _currentThrowForce = value; } }
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
        projectileSpawnPoint = ComponentFinder.GetComponentInChildrenByNameAndType<Transform>("FirePointSprite");
        EventSystem.current.onThrowWeaponRelease += ThrowWeapon;
    }

    private void OnDestroy()
    {
        EventSystem.current.onThrowWeaponRelease -= ThrowWeapon;
    }

    // tracks data around clicks and time held
    public void Execute(string inputState, string currentControlScheme)
    {
        if (inputState == "Button Clicked")
        {
            if (playerSecondaryWeapon.currentWeapon.isThrown)
            {
                InActiveThrow = true;
                secondaryWeaponsManager.ChangingIsBlocked = true;
                if (currentControlScheme == "Keyboard and Mouse") { throwMouseStartingPos = gameController.playerPosition; }
                if (currentControlScheme == "Gamepad") { throwMouseStartingPos = gameController.playerPositionScreen; } 
            } 
        }
        else if (inputState == "Button Released")
        {
            throwKeyReleased = Time.time;
            if (InActiveThrow) { animator.Play("PlayerThrow"); }
        }
        if (inputState == "Button Held")
        {
            if (InActiveThrow)
            {
                if (currentControlScheme == "Keyboard and Mouse") { throwMouseFinishingPos = gameController.lookInputWorld; }
                throwDistanceToPass = Vector2.Distance(throwMouseFinishingPos, throwMouseStartingPos) * throwMultiplier;
                CurrentThrowForce = CalcThrowForce(throwDistanceToPass);
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

    public void ThrowWeapon()
    {
        // instantiate thrown weapon
        GameObject toss = Instantiate(playerSecondaryWeapon.projectilesToUse[playerSecondaryWeapon.currentAmmoIndex], projectileSpawnPoint.position, projectileSpawnPoint.transform.rotation);

        toss.GetComponent<ProjectileBase>().projectile = playerSecondaryWeapon.MostRecentProjectile;

        // get relevant data
        var projectile = toss.GetComponent<ProjectileBase>().projectile;

        // set direction
        Vector3 bulletDir = ((Vector3)gameController.lookInput - (Vector3)gameController.playerPositionScreen).normalized;
        
        // play toss audio
        FindObjectOfType<AudioManager>().PlaySFX(projectile.audioOnUse);

        // give proper gravity
        toss.GetComponent<Rigidbody2D>().gravityScale = projectile.startingGravityScale;

        if (10 > transform.rotation.eulerAngles.z && transform.rotation.eulerAngles.z > -10)
        {
            toss.GetComponent<Rigidbody2D>().AddForce(bulletDir * CurrentThrowForce, ForceMode2D.Impulse);
        }
        else { toss.GetComponent<Rigidbody2D>().AddForce( bulletDir * CurrentThrowForce, ForceMode2D.Impulse); }

        // reset conditions
        InActiveThrow = false;
        secondaryWeaponsManager.ChangingIsBlocked = false;
        EventSystem.current.FinishChargedAttackTrigger();
    }
}
