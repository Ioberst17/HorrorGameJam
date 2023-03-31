using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float healthChecker;
    public float HP_Max;
    private EnemyController enemyController;

    private UIHealthChangeDisplay display;
    [SerializeField] private CanvasGroup healthSliderUI;
    [SerializeField] private Image healthSliderFill;

    // Start is called before the first frame update
    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        display = GetComponentInChildren<UIHealthChangeDisplay>();
        healthSliderUI = GetComponentInChildren<CanvasGroup>();
        healthSliderFill = healthSliderUI.gameObject.GetComponentInChildren<Image>();
        healthSliderUI.alpha = 0;
        HP_Max = enemyController.HP_MAX;
        healthChecker = enemyController.HP_MAX;
    }

    private void Update() 
    { 
        healthSliderUI.transform.rotation = Quaternion.Euler(0, 0, -90); // lock Rotation

        if(display != null)
        {
            if (healthChecker > enemyController.HP && healthChecker <= HP_Max) { display.ShowChange(healthChecker - enemyController.HP, "Negative"); }
            else if (healthChecker < enemyController.HP) { display.ShowChange(enemyController.HP - healthChecker, "Positive"); }

            healthChecker = enemyController.HP;
        }
    }

    public void UpdateHealthUI(int HP)
    {
        healthSliderUI.alpha = 1;
        healthSliderFill.fillAmount = (float)HP/HP_Max;

        if (HP <= 0) { Destroy(gameObject, 0.1f); }
    }
}
