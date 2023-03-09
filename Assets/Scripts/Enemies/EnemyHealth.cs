using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float HPMax;
    private EnemyController enemyController;

    [SerializeField] private CanvasGroup healthSliderUI;
    [SerializeField] private Image healthSliderFill;

    // Start is called before the first frame update
    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        healthSliderUI = GetComponentInChildren<CanvasGroup>();
        healthSliderFill = healthSliderUI.gameObject.GetComponentInChildren<Image>();
        healthSliderUI.alpha = 0;
        HPMax = enemyController.HP;
    }

    private void Update() { healthSliderUI.transform.rotation = Quaternion.Euler(0, 0, -90); }

    public void UpdateHealthUI(int HP)
    {
        healthSliderUI.alpha = 1;
        healthSliderFill.fillAmount = (float)HP/HPMax;

        if (HP <= 0) { Destroy(gameObject, 0.1f); }
    }
}
