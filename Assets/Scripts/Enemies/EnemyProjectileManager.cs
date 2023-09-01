using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyProjectileManager : ProjectileManager
{
    EnemyDataLoader enemyData;
    EnemyController enemyController;
    [SerializeField] Animator animator;
    [SerializeField] int instanceID;

    public string projectilePath;
    private Coroutine chargeCoroutine;

    private void Awake()
    {
        enemyData = GetComponentInParent<EnemyDataLoader>();
        if (enemyData != null) { enemyData.DataLoaded += BuildProjectileInfoDictionaries; }
        EventSystem.current.projectileLaunch += UseTargetedSpell;
    }

    private void OnDestroy() 
    { 
        enemyData.DataLoaded -= BuildProjectileInfoDictionaries;
        EventSystem.current.projectileLaunch -= UseTargetedSpell;
    }


    protected override void Start()
    {
        enemyController = GetComponentInParent<EnemyController>();
        instanceID = transform.parent.gameObject.GetInstanceID();
        animator = ComponentFinder.GetComponentInChildrenByNameAndType<Animator>("Animator");
        projectileSpawnPoint = transform.Find("ProjectileSpawnPoint");
        BuildProjectileInfoDictionaries();

        if (projectilePath == "") { projectilePath = projectiles[0].ownerNoSpace; }

        base.Start();
    }

    protected void FixedUpdate()
    {
        // update time since last fire for all projectiles
        projectileFramesSinceLastShot.IncrementAllValues();
    }

    /// <summary>
    /// Stores information that will be loaded as needed into projectiles fired
    /// </summary>
    protected override void BuildProjectileInfoDictionaries()
    {
        projectiles = enemyData.projectiles;
        base.BuildProjectileInfoDictionaries();
    }
    /// <summary>
    /// Loads the game objects that will be fired in the scene
    /// </summary>
    protected override void LoadProjectileObjects()
    {
        // load ammo prefabs to a list
        projectilesToUse = Resources.LoadAll<GameObject>("EnemyProjectiles/" + projectilePath).ToList();

        // go through the list and sort them in order by ammo IDs
        // projectilesToUse.Sort((randomAmmo, ammoToCompareTo) => randomAmmo.GetComponent<Ammo>().GetAmmoID().CompareTo(ammoToCompareTo.GetComponent<Ammo>().GetAmmoID()));
    }

    override public void Shoot(GameObject ammoToUse, Vector3 target = default(Vector3))
    {
        var referenceID = ammoToUse.GetComponent<ProjectileBase>().projectile.referenceID;
        if (HasNotExceededFireRate(referenceID))
        {
            projectileFramesSinceLastShot[referenceID] = 0;
            base.Shoot(ammoToUse, target);
        }
    }

    public void StartChargeAttack(int referenceID, string animationName = "")
    {
        if(HasNotExceededFireRate(referenceID))
        {
            projectileFramesSinceLastShot[referenceID] = 0;
            enemyController.IsAttacking = true;

            CacheMostRecentProjectile(referenceID);
            if (animationName != "") { animator.Play(animationName); }
        } 
        
        //// Stop any ongoing charge coroutine
        //if (chargeCoroutine != null) { StopCoroutine(chargeCoroutine); } 

        //chargeCoroutine = StartCoroutine(ChargeAttackCoroutine());
    }

    override protected void UseTargetedSpell(int instanceID)
    {
        if (transform.parent.gameObject.GetInstanceID() == instanceID)
        {
            GameObject projectileToUse = projectilesToUse.Find(proj =>
                                                                proj.GetComponent<ProjectileBase>().referenceID == MostRecentProjectile.referenceID);

            projectileToUse.GetComponent<ProjectileBase>().projectile = MostRecentProjectile;

            // update position with any targeting modifiers e.g. adjust up from the player location
            var positionToUse = ReturnModifiedPosition(GetComponentInParent<EnemyController>().playerLocation.position,
                                                                                               MostRecentProjectile.targetingModifierX,
                                                                                               MostRecentProjectile.targetingModifierY);

            // instantiate 
            GameObject projectile = Instantiate(projectileToUse,
                                                positionToUse,
                                                Quaternion.identity);

            enemyController.IsAttacking = false;
        }
    }

    //public void InterruptChargeAttack()
    //{
    //    if (chargeCoroutine != null)
    //    {
    //        StopCoroutine(chargeCoroutine);
    //        // Perform quit logic immediately
    //        QuitLogic();
    //    }
    //}

    //private IEnumerator ChargeAttackCoroutine()
    //{
    //    yield return null; //new WaitForSeconds(chargeTime);

    //    // At this point, the charge attack has finished
    //    // Perform the charge attack logic
    //    PerformChargeAttack();
    //}

    //private void PerformChargeAttack()
    //{
    //    // Perform charge attack logic here
    //}

    //private void QuitLogic()
    //{
    //    // Perform your quit logic here
    //}
}
