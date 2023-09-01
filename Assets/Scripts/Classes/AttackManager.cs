using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ComponentFinder;

/// <summary>
/// Stores basic functions related to melee attacks; used by both player and enemy melee managers to set up a hitbox
/// </summary>
public class AttackManager : MonoBehaviour
{
    //These are all the objects used for physical hit detection
    [SerializeField] protected GameObject HitBox;
    [SerializeField] protected Transform hitBoxPoint1, hitBoxPoint2;

    // used to build the dictionaries: list of attacks and information about them including hitbox information,
    // Data is stored in a text file in resources, and loaded in by the child class (enemies load from EnemyAttackDatabase, player from PlayerAttackDatabase)
    protected List<Attack> attacks = new();
    // used for simplicity of access to hitbox data
    public SimpleSerializableDictionary<string, HitBox> attackDimensions = new();
    // stores all attack information
    public SimpleSerializableDictionary<string, Attack> attackDictionary = new();
    // cached information to pass to shields
    [SerializeField] private Attack _mostRecentAttack; public Attack MostRecentAttack { get { return _mostRecentAttack; } set { _mostRecentAttack = value; } }

    //List of objects hit by an attack, used to let the player hit multiple things with one swing
    [SerializeField] protected ContactFilter2D normalCollisionFilter = new ();
    [SerializeField] protected List<Collider2D> hitList = new();
    protected int hitListLength;

    // set using attackBuffer from the the attack database; it is the number of frames before another attack can be attempted
    [SerializeField] protected float _attackLagTimer; virtual public float AttackLagTimer { get { return _attackLagTimer; } set { _attackLagTimer = value; } }

    virtual protected void Start()
    {
        HitBox = GetComponentInChildren<HitBoxVisualizer>(true).gameObject;
        HitBox.SetActive(true);
        hitBoxPoint1 = HitBox.transform.Find("Point1");
        hitBoxPoint2 = HitBox.transform.Find("Point2");
        HitBox.SetActive(false);

        AddLayersToCheck();
    }

    virtual protected void FixedUpdate()
    {
        if (AttackLagTimer > 0) { AttackLagTimer -= 1; }
        CollisionCheck();
    }

    virtual public void StartAttack(int attackDirection, string animationToPlay) { }    
    
    virtual public void StartChargeAttack(int attackDirection, string animationToPlay, string successAnimationToPlay, IEnumerator AdditionalEndConditions = null) { }

    virtual protected IEnumerator ReleaseChargeAttack(Attack currentAttack, string animationToPlay, string successAnimationToPlay, IEnumerator AdditionalReleaseConditions = null) { yield return null; }

    /// <summary>
    /// Grabbed by attacked object's shield to calculate damage calculations
    /// </summary>
    /// <param name="animationToPlay"></param>
    protected void CacheMostRecentAttack(string animationToPlay)
    {
        // cache most recent attack
        if (attackDictionary.TryGetValue(animationToPlay, out _mostRecentAttack))
        {
            MostRecentAttack = attackDictionary[animationToPlay];
        }
        else
        {
            Debug.Log("Animation named: " + animationToPlay + " does not exist in the attack dictionary; " +
                        "check if the animation name used matches the nameInEngine field of the attacks list");
        }
    }

    virtual protected void CollisionCheck()
    {
        if (HitBox.activeSelf) { CheckForCollisions(hitBoxPoint1.position, hitBoxPoint2.position); }
    }

    virtual protected void CheckForCollisions(Vector2 point1, Vector2 point2)
    {
        hitListLength = Physics2D.OverlapArea(point1, point2, normalCollisionFilter, hitList);
        if (hitListLength > 0)
        {
            int i = 0;
            while (i < hitList.Count)
            {                
                if (hitList[i].GetComponent<IDamageable>() != null && hitList[i])
                {
                    PassBreakableObjectDamage(hitList[i]);
                    AdditionalAbilitySpecificChecksDamagable(hitList[i]);
                }
                else if (hitList[i].gameObject.layer == LayerMask.NameToLayer("Environment") && hitList[i].gameObject.tag == "Boundary")
                {
                    AdditionalAbilitySpecificChecksNonDamagable(hitList[i]);
                }
                i++;
            }
        }
    }

    /// <summary>
    /// Must be clarified in the child class
    /// </summary>
    virtual protected void AddLayersToCheck() { }
    virtual protected void BuildAttackInfoDictionaries()
    {
        // get a list of attack dimensions
        foreach (var entry in attacks)
        {
            attackDictionary.Add(entry.nameInEngine, entry);
            attackDimensions.Add(entry.nameInEngine, new HitBox(new Vector3(entry.hitBoxPoint1X, entry.hitBoxPoint1Y), 
                                                                new Vector3(entry.hitBoxPoint2X, entry.hitBoxPoint2Y)));
        }
    }

    virtual protected void LoadHitBox(string animationToPlay)
    {
        if (attackDimensions.TryGetValue(animationToPlay, out HitBox value))
        {
            hitBoxPoint1.transform.localPosition = value.point1;
            hitBoxPoint2.transform.localPosition = value.point2;
        }
        else { Debug.Log("Key '" + animationToPlay + "' not found in the dictionary: " + attackDimensions); }
    }

    virtual protected void HitBoxOn() { }
    virtual protected void HitBoxOff() { }

    virtual protected void PassBreakableObjectDamage(Collider2D breakableObject) 
    {
        if (breakableObject.GetComponent<Breakable>() != null) { breakableObject.GetComponent<Breakable>().Hit(); }
    }

    virtual protected void AdditionalAbilitySpecificChecksDamagable(Collider2D hitObject) { }

    virtual protected void AdditionalAbilitySpecificChecksNonDamagable(Collider2D hitObject) { }
}
