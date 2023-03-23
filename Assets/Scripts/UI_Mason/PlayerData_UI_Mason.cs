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
    [SerializeField] private Image mpBar;
    [SerializeField] private Image spBar;
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
    public float mp;
    public float sp;

    [SerializeField] private DataManager dataManager;

    [SerializeField] private GameController gameController;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerHealth playerHealth;

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
        FinishTossForceDisplay();
        healthChecker = gameController.GetHP();
        healthBar.fillAmount = health / 100f;

        //dataManager.gameData.consumables[1].amount = dataManager.gameData.consumables[1].amount + 2;

        /*throwPredictionPoints = new GameObject[numberOfThrowPoints];

        for(int i = 0; i < numberOfThrowPoints; i++) { throwPredictionPoints[i] = Instantiate(throwPredictionPoint, transform.position, Quaternion.identity); }*/
    }

    void Update()
    {
        if(healthChecker >  gameController.GetHP() && healthChecker <= playerHealth.maxHealth) { display.ShowChange(healthChecker - gameController.GetHP(), "Negative"); }
        else if (healthChecker < gameController.GetHP()) { display.ShowChange(gameController.GetHP() - healthChecker, "Positive"); }

        healthChecker = gameController.GetHP();
        health = gameController.GetHP();
        healthBar.fillAmount = health / 100f; //can import the max health to make this better but as for right now the hp is 100

        mp = gameController.GetMP();
        mpBar.fillAmount = mp / 100f;

        sp = gameController.GetSP();
        spBar.fillAmount = sp / 100f;

        /*consumableAmount.text = dataManager.gameData.consumables[1].amount.ToString();

        if (Input.GetKeyDown(KeyCode.H) && dataManager.gameData.consumables[1].amount > 0 && gameController.GetHP() < 100)
        {
            playerHealth.AddHealth(10);
            dataManager.gameData.consumables[1].amount = dataManager.gameData.consumables[1].amount - 1;
            Debug.Log("Used health kit.\n");
        }*/
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
        /*
        if(throwPredictionPoints != null)
        { for (int i = 0; i < throwPredictionPoints.Length; i++) { TossPredictionVisibility(false, throwPredictionPoints[i]); } }*/
    }

    private void OnDestroy()
    {
        // unsubscribe from events
        EventSystem.current.onUpdateSecondaryWeaponUITrigger -= UpdateAmmoUI;
        EventSystem.current.onStartChargingUITrigger -= StartTossForceDisplay;
        EventSystem.current.onFinshChargingUITrigger -= FinishTossForceDisplay;
    }

}
