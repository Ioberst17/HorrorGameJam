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

    [SerializeField] private Canvas ThrowForceUI;
    [SerializeField] private Image ThrowForceFill;
    Transform StartingTransform;
    public GameObject throwPredictionPoint;
    private GameObject[] throwPredictionPoints;
    private int numberOfThrowPoints = 30;

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

        throwPredictionPoints = new GameObject[numberOfThrowPoints];

        for(int i = 0; i < numberOfThrowPoints; i++) { throwPredictionPoints[i] = Instantiate(throwPredictionPoint, transform.position, Quaternion.identity); }
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

    private void StartTossForceDisplay(float normalizedTossForce, Transform tossSpawnPoint, float tossForce)
    {
        ShowTossStrengthUI(normalizedTossForce);
        ShowTossTrajectory(tossSpawnPoint, tossForce);
    }

    private void ShowTossStrengthUI(float tossForce)
    {
        ThrowForceUI.GetComponent<CanvasGroup>().alpha = 1;
        ThrowForceFill.GetComponent<Image>().fillAmount = tossForce;
        ThrowForceUI.transform.rotation = Quaternion.Euler(0, 0, 0);
        ThrowForceFill.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void ShowTossTrajectory(Transform tossSpawnPoint, float tossForce)
    {
        for(int i = 0; i < throwPredictionPoints.Length; i++)
        {
            TossPredictionVisibility(true, throwPredictionPoints[i]);
            throwPredictionPoints[i].transform.position = CalcPointPositions(i * 0.05f, tossSpawnPoint, tossForce);
        }
    }

    Vector2 CalcPointPositions(float time, Transform tossSpawnPoint, float tossForce)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 transformPos = tossSpawnPoint.position;
        mousePos.z = transformPos.z;
        Vector3 bulletDir = (mousePos - transformPos).normalized;

        Vector2 currentPointPosition = (Vector2)tossSpawnPoint.transform.position + (Vector2)(time * tossForce * bulletDir) + (time * time) * 0.5f * Physics2D.gravity;
        return currentPointPosition;
    }

    private void TossPredictionVisibility(bool OnOrOff, GameObject tossPoint)
    {
        if(OnOrOff == true)
        {
            Color temp = tossPoint.GetComponent<SpriteRenderer>().color;
            temp.a = 1f;
            tossPoint.GetComponent<SpriteRenderer>().color = temp;
        }
        else if (OnOrOff == false)
        {
            Color temp = tossPoint.GetComponent<SpriteRenderer>().color;
            temp.a = 0f;
            tossPoint.GetComponent<SpriteRenderer>().color = temp;
        }
        else { }
    }

    private void FinishTossForceDisplay()
    {
        ThrowForceUI.GetComponent<CanvasGroup>().alpha = 0;
        ThrowForceFill.GetComponent<Image>().fillAmount = 0;
        if(throwPredictionPoints != null)
        { for (int i = 0; i < throwPredictionPoints.Length; i++) { TossPredictionVisibility(false, throwPredictionPoints[i]); } }      
    }

    private void OnDestroy()
    {
        // unsubscribe from events
        EventSystem.current.onUpdateWeaponUITrigger -= UpdateAmmoUI;
        EventSystem.current.onStartTossingTrigger -= StartTossForceDisplay;
        EventSystem.current.onFinishTossingTrigger -= FinishTossForceDisplay;
    }

}
