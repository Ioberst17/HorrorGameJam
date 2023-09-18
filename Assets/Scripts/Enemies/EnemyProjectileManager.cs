using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
        EventSystem.current.projectileLaunch += DirectShotFromAnimation;
        EventSystem.current.targetedSpellTrigger += UseTargetedSpell;
    }

    private void OnDestroy() 
    { 
        enemyData.DataLoaded -= BuildProjectileInfoDictionaries;
        EventSystem.current.projectileLaunch -= DirectShotFromAnimation;
        EventSystem.current.targetedSpellTrigger -= UseTargetedSpell;
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

    /// <summary>
    /// Called for the case of direct shots and has an overload for targeted thrown shots
    /// </summary>
    /// <param name="ammoToUse"></param>
    /// <param name="target"></param>
    override public void ShootHandler(GameObject ammoToUse, Vector3 target = default(Vector3))
    {
        var referenceID = ammoToUse.GetComponent<ProjectileBase>().referenceID;

        if (HasNotExceededFireRate(referenceID))
        {
            projectileFramesSinceLastShot[referenceID] = 0;
            base.ShootHandler(ammoToUse, target);
        }
    }

    /// <summary>
    /// Called to start an attack that launches on an animation frame; requires a projectile reference ID from the relevant database. 
    /// Optionally, can input an animation name to play with projectile
    /// </summary>
    /// <param name="referenceID"></param>
    /// <param name="animationName"></param>
    public void Shoot(int referenceID, string animationName)
    {
        if(HasNotExceededFireRate(referenceID))
        {
            // reset shot frames to 0
            projectileFramesSinceLastShot[referenceID] = 0;

            // important to cache so projectile manager has information to launch with later in the animation
            CacheMostRecentProjectile(referenceID);
            animator.Play(animationName); 
        } 
    }

    /// <summary>
    ///  This is called from a keyframe in an animation
    /// </summary>
    /// <param name="instanceID"></param>
    override protected void DirectShotFromAnimation(int instanceID)
    {
        if (transform.parent.gameObject.GetInstanceID() == instanceID)
        {
            // reset shot frames to 0
            projectileFramesSinceLastShot[MostRecentProjectile.referenceID] = 0;

            base.DirectShotFromAnimation(instanceID);
            //enemyController.IsAttacking = false;
        }
    }

    override protected void UseTargetedSpell(int instanceID)
    {
        if (transform.parent.gameObject.GetInstanceID() == instanceID)
        {
            base.UseTargetedSpell(instanceID);

            //enemyController.IsAttacking = false;
        }
    }
}
