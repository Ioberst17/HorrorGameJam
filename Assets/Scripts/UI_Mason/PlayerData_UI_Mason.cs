using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;



public class PlayerData_UI_Mason : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    //[SerializeField] private Image mpBar;
    //[SerializeField] private Image spBar;
    [SerializeField] private TextMeshProUGUI weaponName;
    [SerializeField] private TextMeshProUGUI weaponAmmo;
    [SerializeField] private TextMeshProUGUI consumableAmount;

    [SerializeField] private Canvas ThrowForceUI;
    [SerializeField] private Image ThrowForceFill;
    [SerializeField] private UIHealthChangeDisplay display;
    Transform StartingTransform;
    public GameObject throwPredictionPoint;
    private GameObject[] throwPredictionPoints;
    private int numberOfThrowPoints = 30;

    public float healthChecker;
    public float health;


    [SerializeField] private DataManager dataManager;
    GameController gameController;
    [SerializeField] private PlayerController playerController;
    private PlayerHealth playerHealth;
    //private PlayerStamina playerStamina;
    //private PlayerMana playerMana;

    [SerializeField] private PlayerDash playerDash;

    private int dashCoolDownTime;
    [SerializeField] private Image dashCircle;
    [SerializeField] private Image dashImage;
    [SerializeField] private Image dashBackground;

    private int dashTimerInverse;

    // Start is called before the first frame update
    void Awake()
    {
        //subscribe to events
        EventSystem.current.onUpdateSecondaryWeaponUITrigger += UpdateAmmoUI;
        EventSystem.current.onStartChargingUITrigger += StartTossForceDisplay;
        EventSystem.current.onFinshChargingUITrigger += FinishTossForceDisplay;
    }

    private void Start()
    {

        dataManager = DataManager.Instance;

        FinishTossForceDisplay();
        gameController = FindObjectOfType<GameController>();
        playerHealth = playerController.GetComponent<PlayerHealth>();
        playerDash = playerController.GetComponentInChildren<PlayerDash>();
        //playerStamina = playerController.GetComponentInChildren<PlayerStamina>();
        //playerMana = playerController.GetComponentInChildren<PlayerMana>();

        healthChecker = playerHealth.HP;
        healthBar.fillAmount = playerHealth.HP / 100f;

        dashCoolDownTime = playerDash.dashcooldown;
        /*dashCircle.enabled = false;
        dashImage.enabled = false;
        dashBackground.enabled = false;*/

        TurnOnDashUI();


        //dataManager.sessionData.consumables[1].amount = dataManager.sessionData.consumables[1].amount + 2;

        /*throwPredictionPoints = new GameObject[numberOfThrowPoints];

        for(int i = 0; i < numberOfThrowPoints; i++) { throwPredictionPoints[i] = Instantiate(throwPredictionPoint, transform.position, Quaternion.identity); }*/
    }

    void Update()
    {
        if(healthChecker >  playerHealth.HP && healthChecker <= playerHealth.maxHealth) { display.ShowChange(healthChecker - playerHealth.HP, "Negative"); }
        else if (healthChecker < playerHealth.HP) { display.ShowChange(playerHealth.HP - healthChecker, "Positive"); }

        healthChecker = playerHealth.HP;
        health = playerHealth.HP;
        healthBar.fillAmount = health / 100f; //can import the max health to make this better but as for right now the hp is 100

       // mpBar.fillAmount = playerMana.MP / 100f;

        //spBar.fillAmount = playerStamina.SP / 100f;
        if(dataManager.sessionData.consumables.Count >= 2)
        {
            if (dataManager.sessionData.consumables[1].amount > 0)
            {
                consumableAmount.text = dataManager.sessionData.consumables[1].amount.ToString();
                
            }
            
        }

        dashTimerInverse = 40 - playerDash.dashcooldown;


        //dashCoolDownTime = playerDash.dashcooldown;
        dashCircle.fillAmount = dashTimerInverse / 40f;

        /*if (dashCoolDownTime > 0)
        {
            TurnOnDashUI();
        }
        else { TurnOffDashUI(); }*/

    }

    public void UseHealthPack()
    {
        if(dataManager.sessionData.consumables != null && 1 < dataManager.sessionData.consumables.Count && dataManager.sessionData.consumables[1] != null)
        {
            if (dataManager.sessionData.consumables[1].amount > 0 && playerHealth.HP < 100)
            {
                playerHealth.AddHealth(10);
                dataManager.sessionData.consumables[1].amount = dataManager.sessionData.consumables[1].amount - 1;
                Debug.Log("Used health kit.\n");
            }
        }
    }

    public void UpdateAmmoUI(string updatedWeapon, int updatedAmmo)
    {
        weaponName.text = updatedWeapon;
        weaponAmmo.text = updatedAmmo.ToString();
    } 

    private void StartTossForceDisplay(float normalizedTossForce, Transform tossSpawnPoint, float? tossForce)
    {
        ShowTossStrengthUI(normalizedTossForce);
        if(tossForce != null) PlayerThrowPredictionPointsObjectPool.Instance.ShowTossTrajectory(tossSpawnPoint, tossForce);
    }

    private void ShowTossStrengthUI(float tossForce)
    {
        ThrowForceUI.GetComponent<CanvasGroup>().alpha = 1;
        ThrowForceFill.GetComponent<Image>().fillAmount = tossForce;
        ThrowForceUI.transform.rotation = Quaternion.Euler(0, 0, 0);
        ThrowForceFill.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void FinishTossForceDisplay()
    {
        ThrowForceUI.GetComponent<CanvasGroup>().alpha = 0;
        ThrowForceFill.GetComponent<Image>().fillAmount = 0;
        PlayerThrowPredictionPointsObjectPool.Instance.ClearToss();
    }

    private void OnDestroy()
    {
        // unsubscribe from events
        EventSystem.current.onUpdateSecondaryWeaponUITrigger -= UpdateAmmoUI;
        EventSystem.current.onStartChargingUITrigger -= StartTossForceDisplay;
        EventSystem.current.onFinshChargingUITrigger -= FinishTossForceDisplay;
    }


    public void TurnOnDashUI()
    {
        dashImage.enabled = true;
        dashCircle.enabled = true;
        dashBackground.enabled = true;
    }
    public void TurnOffDashUI()
    {
        dashImage.enabled = false;
        dashCircle.enabled = false;
        dashBackground.enabled = false;
    }

}
