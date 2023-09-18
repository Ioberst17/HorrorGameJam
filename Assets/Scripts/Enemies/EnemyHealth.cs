using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerAttackManager;

// Handles the concept of health and its display
public class EnemyHealth : Health
{
    EnemyDataLoader enemyData;
    EnemyController enemyController;
    WeaponDatabase weaponDatabase;
    EnemyLoot enemyLoot;

    public int healthChecker;
    [SerializeField] float healthDifficultyMultiplier;

    public event Action<EnemyController> OnDeath;

    // Handles UI Display
    UIHealthChangeDisplay display;
    [SerializeField] CanvasGroup healthSliderUI;
    [SerializeField] Image healthSliderFill;

    void Start()
    {
        // External References
        enemyLoot = GetComponentInChildren<EnemyLoot>();
        enemyController = GetComponent<EnemyController>();
        display = GetComponentInChildren<UIHealthChangeDisplay>();
        healthSliderUI = GetComponentInChildren<CanvasGroup>();
        healthSliderFill = healthSliderUI.gameObject.GetComponentInChildren<Image>();
        weaponDatabase = FindObjectOfType<WeaponDatabase>();

        // Set values
        healthSliderUI.alpha = 0;
    }

    private void Awake()
    {
        enemyData = GetComponentInParent<EnemyDataLoader>();
        if (enemyData != null) { enemyData.DataLoaded += LoadInValues; }

        // Load events
        EventSystem.current.onEnemyEnviroDamage += Hit;
        EventSystem.current.onEnemyAmmoHitCollision += Hit;
        EventSystem.current.onEnemyMeleeHitCollision += Hit;
        EventSystem.current.onEnemyParryCollision += Hit;
    }

    private void OnDestroy()
    {
        // unsubscribe from events
        enemyData.DataLoaded -= LoadInValues;
        EventSystem.current.onEnemyEnviroDamage -= Hit;
        EventSystem.current.onEnemyAmmoHitCollision -= Hit;
        EventSystem.current.onEnemyMeleeHitCollision -= Hit;
        EventSystem.current.onEnemyParryCollision -= Hit;
    }

    void LoadInValues()
    {
        // assume medium, but otherwise get the difficulty level from player prefs
        string difficultyLevel = "Medium";
        if (PlayerPrefs.HasKey("DifficultyLevel")) { difficultyLevel = PlayerPrefs.GetString("DifficultyLevel"); }

        // set multipliers based on value
        if (difficultyLevel == "Easy") { healthDifficultyMultiplier = enemyData.data.easyHPMultiplier;  }
        else if (difficultyLevel == "Hard") { healthDifficultyMultiplier = enemyData.data.hardHPMultiplier; }
        else { healthDifficultyMultiplier = enemyData.data.mediumHPMultiplier; }

        (MaxHealth, HP) = ((int)(enemyData.data.health * healthDifficultyMultiplier), (int)(enemyData.data.health * healthDifficultyMultiplier));
    }


    // VARIOUS OVERRIDES OF WAYS ENEMY CAN BE HIT
    public void Hit(int attackDamage, Vector3 playerPosition, string statusEffect, EnemyController enemyController)
    {
        if (!enemyController.IsDead)
        {
            if (enemyController == GetComponent<EnemyController>() || enemyController == null) // latter condition is used by PlayerPrimaryWeapon
            {
                if (!enemyController.IsInvincible)
                {
                    DamageInterrupt = true; 
                    Debug.Log("Hit is being called");
                    if (statusEffect != null) { enemyController.StatusModifier(statusEffect); }
                    TakeDamage(attackDamage);
                    enemyController.HandleHitPhysics(playerPosition, 0);
                    StartCoroutine(enemyController.HitStun());
                }
            }
        }
    }

    // used by shots / ammo
    public void Hit(int damageToPass, int LevelOfWeapon, Vector3 playerPosition, string statusEffect, EnemyController enemyController) 
    {
        Hit(damageToPass, playerPosition, statusEffect, enemyController);
    }

    // used by parry (shield mechanic)
    public void Hit(int damageValue, Vector3 playerPosition, GameObject enemyObject)
    {
        if (enemyObject.TryGetComponent(out EnemyController enemy))
        {
            var damageToPass = damageValue;
            Hit(damageToPass, playerPosition, null, enemy);
        }   
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;
        UpdateHealthUI(HP);
        Debug.Log("Enemy " + gameObject.name + " was damaged! It took: " + damage + "damage. It's current HP is: " + HP);
        if (HP <= 0) { HPZero(); }
    }

    override public void HPZero()
    {
        Instantiate(Resources.Load("VFXPrefabs/EnemyDeath"), transform.position, Quaternion.identity);
        Debug.Log(name + " is dead!");
        enemyController.HitBox.enabled = false;
        enemyController.SpriteRenderer.enabled = false;
        enemyController.IsDead = true;
        if (OnDeath != null) { OnDeath(enemyController); }
        string enemyDeathSound = gameObject.tag.ToString() + "Death"; // creates string that AudioManager recognizes; tag should match asset in AudioManager
        FindObjectOfType<AudioManager>().PlaySFX(enemyDeathSound);
        enemyLoot.InstantiateLoot(transform.position);
        Destroy(gameObject);
    }

    // PRIMARILY UI-RELATED BELOW

    private void Update() 
    { 
        healthSliderUI.transform.rotation = Quaternion.Euler(0, 0, -90); // lock Rotation

        if(display != null)
        {
            if (healthChecker > HP && healthChecker <= MaxHealth) { display.ShowChange(healthChecker - HP, "Negative"); }
            else if (healthChecker < HP) { display.ShowChange(HP - healthChecker, "Positive"); }

            healthChecker = HP;
        }
    }

    public void UpdateHealthUI(int HP)
    {
        healthSliderUI.alpha = 1;
        healthSliderFill.fillAmount = (float)HP/MaxHealth;

        if (HP <= 0) { Destroy(gameObject, 0.1f); }
    }
}
