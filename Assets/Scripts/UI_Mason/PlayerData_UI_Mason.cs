using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class PlayerData_UI_Mason : MonoBehaviour
{

    [SerializeField] private Image healthBar;
    [SerializeField] private Image mpBar;
    [SerializeField] private Image spBar;

    public float health;
    public float mp;
    public float sp;

    [SerializeField] private GameController gameController;

    [SerializeField] private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        health = gameController.GetHP();
        healthBar.fillAmount = health / 100f; //can import the max health to make this better but as for right now the hp is 100

        mp = gameController.GetMP();
        mpBar.fillAmount = mp / 100f;

        sp = gameController.GetSP();
        spBar.fillAmount = sp / 100f;
    }
}
