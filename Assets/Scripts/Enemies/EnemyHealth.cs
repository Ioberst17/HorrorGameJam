using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerPrimaryWeapon;

// Handles the concept of health and its display
public class EnemyHealth : Health
{
    private EnemyController enemyController;
    WeaponDatabase weaponDatabase;

    public int healthChecker;
    private int damageToPass;

    public event Action<EnemyController> OnDeath;

    public bool damageInterupt = false;

    // Handles UI Display
    private UIHealthChangeDisplay display;
    [SerializeField] private CanvasGroup healthSliderUI;
    [SerializeField] private Image healthSliderFill;

    void Start()
    {
        // External References
        enemyController = GetComponent<EnemyController>();
        display = GetComponentInChildren<UIHealthChangeDisplay>();
        healthSliderUI = GetComponentInChildren<CanvasGroup>();
        healthSliderFill = healthSliderUI.gameObject.GetComponentInChildren<Image>();
        weaponDatabase = GameObject.Find("WeaponDatabase").GetComponent<WeaponDatabase>();

        // Set values
        healthSliderUI.alpha = 0;

        EventSystem.current.onEnemyEnviroDamage += Hit;
        EventSystem.current.onEnemyAmmoHitCollision += Hit;
        EventSystem.current.onEnemyMeleeHitCollision += Hit;
    }

    private void OnDestroy()
    {
        // unsubscribe from events
        EventSystem.current.onEnemyEnviroDamage -= Hit;
        EventSystem.current.onEnemyAmmoHitCollision -= Hit;
        EventSystem.current.onEnemyMeleeHitCollision -= Hit;
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
                    damageInterupt = true; 
                    Debug.Log("Hit is being called");
                    if (statusEffect != null) { enemyController.StatusModifier(statusEffect); }
                    TakeDamage(attackDamage);
                    enemyController.HandleHitPhysics(playerPosition, 0);
                    StartCoroutine(enemyController.HitStun());
                }
            }
        }
    }

    public void Hit(int weaponID, int LevelOfWeapon, Vector3 playerPosition, string statusEffect, EnemyController enemyController) // used by shots / ammo
    {
        damageToPass = weaponDatabase.GetWeaponDamage(weaponID, LevelOfWeapon);
        statusEffect = weaponDatabase.GetWeaponEffect(weaponID);
        Hit(damageToPass, playerPosition, statusEffect, enemyController);
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
        enemyController.IsDead = true;
        if (OnDeath != null) { OnDeath(enemyController); }
        enemyController.hitBox.enabled = false;
        enemyController.SpriteRenderer.enabled = false;
        string enemyDeathSound = gameObject.tag.ToString() + "Death"; // creates string that AudioManager recognizes; tag should match asset in AudioManager
        FindObjectOfType<AudioManager>().PlaySFX(enemyDeathSound);
        if (GetComponent<EnemyLoot>() != null) { GetComponent<EnemyLoot>().InstantiateLoot(transform.position); }
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
