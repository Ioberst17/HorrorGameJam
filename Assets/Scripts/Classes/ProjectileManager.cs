using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ProjectileManager : MonoBehaviour
{
    // used to build the dictionaries: list of attacks and information about them including hitbox information,
    // Data is stored in a text file in resources, and loaded in by the child class
    protected List<Projectile> projectiles = new();
    // stores all attack information
    public SimpleSerializableDictionary<int, Projectile> projectileDictionary = new();

    // keeps track of time between a projectile and its last shot
    public SimpleSerializableDictionary<int, int> projectileFramesSinceLastShot = new();
    
    // cached information to pass to manage fire rates
    [SerializeField] private Projectile _mostRecentProjectile; public Projectile MostRecentProjectile { get { return _mostRecentProjectile; } set { _mostRecentProjectile = value; } }

    [SerializeField] protected SpriteRenderer weaponSprite;
    public List<GameObject> projectilesToUse;
    public int currentAmmoIndex;

    [SerializeField] public Transform projectileSpawnPoint; // assigned in inspector

    protected virtual void Start()
    {
        LoadProjectileObjects();
    }

    /// <summary>
    /// Must be clarified in the child class
    /// </summary>
    virtual protected void BuildProjectileInfoDictionaries()
    {
        // get a list of attack dimensions
        foreach (var entry in projectiles) 
        { 
            projectileDictionary.Add(entry.referenceID, entry);
            projectileFramesSinceLastShot.Add(entry.referenceID, 0);
        }
    }

    protected virtual void LoadProjectileData() { }

    // defined in child functions
    protected virtual void LoadProjectileObjects() { }

    /// <summary>
    /// Gets a projectile and stores it in MostRecentProjectile using the unique reference ID of a projectile
    /// </summary>
    /// <param name="projectileToUseReferenceID"></param>
    protected void CacheMostRecentProjectile(int projectileToUseReferenceID)
    {
        // cache most recent attack
        if (projectileDictionary.TryGetValue(projectileToUseReferenceID, out _mostRecentProjectile))
        {
            MostRecentProjectile = projectileDictionary[projectileToUseReferenceID];
        }
        else
        {
            Debug.Log("Projectile named: " + projectileToUseReferenceID + " does not exist in the projectile dictionary; " +
                        "check if the projectile name used matches the nameNoSpace field of the projectiles list");
        }
    }

    /// <summary>
    /// Public method: called for regular and targeted shots
    /// </summary>
    /// <param name="ammoToUse"></param>
    virtual public void Shoot(GameObject ammoToUse, Vector3 targetPosition = default(Vector3))
    {
        // cache the most recent projectile using the reference ID
        CacheMostRecentProjectile(ammoToUse.GetComponent<ProjectileBase>().referenceID);

        // instantiate the object
        GameObject shot = Instantiate(ammoToUse, projectileSpawnPoint.position, projectileSpawnPoint.transform.rotation);

        // load the projectile information into the shot
        shot.GetComponent<ProjectileBase>().projectile = MostRecentProjectile;

        if (targetPosition == Vector3.zero) { DirectShot(shot); }
        else { ShootAtTarget(shot, targetPosition); }
    }

    /// <summary>
    /// Shoots a shot forward given the projectile's standard trajectory
    /// </summary>
    private void DirectShot(GameObject shot)
    {
        //use audio on use
        FindObjectOfType<AudioManager>().PlaySFX(MostRecentProjectile.audioOnUse);

        // set the gravity scale
        shot.GetComponent<Rigidbody2D>().gravityScale = MostRecentProjectile.startingGravityScale;

        // add force
        shot.GetComponent<Rigidbody2D>().AddForce(projectileSpawnPoint.transform.right * MostRecentProjectile.launchForceX);
    }

    /// <summary>
    /// Auto-detects a direction before firing; used by projectiles that don't need a specific spawn point, like a gun nozzle vs. generic energy blast
    /// </summary>
    /// <param name="ammoToUse"></param>
    /// <param name="firingPoint"></param>
    /// <param name="target"></param>
    public void ShootAtTarget(GameObject shot, Vector3 target)
    {
        // Calculate the direction to the target
        Vector2 direction = (target - projectileSpawnPoint.position).normalized;

        // Apply force to the projectile in the calculated direction
        Rigidbody2D rb = shot.GetComponent<Rigidbody2D>();
        rb.velocity = GetParabolicVelocityNeeded(direction);
    }

    virtual protected void UseTargetedSpell(int instanceID) 
    {
        if(transform.parent.gameObject.GetInstanceID() == instanceID) 
        {
            GameObject projectileToUse = projectilesToUse.Find(proj => 
                                                                proj.GetComponent<ProjectileBase>().referenceID == MostRecentProjectile.referenceID);

            projectileToUse.GetComponent<ProjectileBase>().projectile = MostRecentProjectile;

            GameObject projectile = Instantiate(projectileToUse, transform.position, Quaternion.identity);
        }
    }


    private Vector3 GetParabolicVelocityNeeded(Vector2 direction)
    {
        // Calculate the time of flight
        float flightTime = (2f * direction.magnitude) / MostRecentProjectile.launchSpeedX;

        // Calculate the initial velocity in 2D (ignoring the y component)
        Vector3 initialVelocity = direction / flightTime;

        // Calculate the required y component of velocity to achieve the desired trajectory
        // this is a physics formula for parabolic flight (velocity * .5 * gravity * time^2) / time
        initialVelocity.y = (direction.y + 0.5f * 9.81f*3  * flightTime * flightTime) / flightTime;

        // return the needed velocity
        return initialVelocity;
    }

    protected Vector3 ReturnModifiedPosition(Vector3 startingPosition, float xModifier, float yModifier)
    {
        startingPosition.x += xModifier; 
        startingPosition.y += yModifier;

        return startingPosition;
    }

    /// <summary>
    /// Compares the proposed projectile against the frames it needs to wait before firing; 
    /// returns true if enough frames have passed, else false
    /// </summary>
    /// <param name="referenceID"></param>
    /// <returns></returns>
    protected bool HasNotExceededFireRate(int referenceID) { return projectileDictionary[referenceID].fireRateFrames < projectileFramesSinceLastShot[referenceID]; }
}
