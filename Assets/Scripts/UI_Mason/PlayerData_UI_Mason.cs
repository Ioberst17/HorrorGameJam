using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class PlayerData_UI_Mason : MonoBehaviour
{

    [SerializeField] private Image healthBar;
    [SerializeField] private Image mpBar;
    [SerializeField] private Image spBar;
    [SerializeField] private TextMeshProUGUI weaponName;
    [SerializeField] private TextMeshProUGUI weaponAmmo;

    [SerializeField] private Canvas ThrowForceUI;
    [SerializeField] private Image ThrowForceFill;

    public float health;
    public float mp;
    public float sp;

    [SerializeField] private GameController gameController;

    [SerializeField] private PlayerController playerController;

    // Start is called before the first frame update
    void Awake()
    {
        //subscribe to events
        EventSystem.current.onUpdateWeaponUITrigger += UpdateAmmoUI;
        EventSystem.current.onStartTossingTrigger += StartTossForceDisplay;
        EventSystem.current.onFinishTossingTrigger += FinishTossForceDisplay;
    }

    private void Start()
    {
        FinishTossForceDisplay();
    }

    void Update()
    {
        health = gameController.GetHP();
        healthBar.fillAmount = health / 100f; //can import the max health to make this better but as for right now the hp is 100

        mp = gameController.GetMP();
        mpBar.fillAmount = mp / 100f;

        sp = gameController.GetSP();
        spBar.fillAmount = sp / 100f;
    }

    private void UpdateAmmoUI(string updatedWeapon, int updatedAmmo)
    {
        weaponName.text = updatedWeapon;
        weaponAmmo.text = updatedAmmo.ToString();
    } 

    private void StartTossForceDisplay(float tossForce)
    {
        ThrowForceUI.GetComponent<CanvasGroup>().alpha = 1;
        ThrowForceFill.GetComponent<Image>().fillAmount = tossForce;
    }

    private void FinishTossForceDisplay()
    {
        ThrowForceUI.GetComponent<CanvasGroup>().alpha = 0;
        ThrowForceFill.GetComponent<Image>().fillAmount = 0;
    }

    private void OnDestroy()
    {
        // unsubscribe from events
        EventSystem.current.onUpdateWeaponUITrigger -= UpdateAmmoUI;
        EventSystem.current.onStartTossingTrigger -= StartTossForceDisplay;
        EventSystem.current.onFinishTossingTrigger -= FinishTossForceDisplay;
    }

}
