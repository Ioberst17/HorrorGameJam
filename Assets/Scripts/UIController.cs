using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameController GameController;
    private PlayerController playerController;
    private PlayerHealth playerHealth;
    private Stamina playerStamina;
    private PlayerMana playerMana;

    [SerializeField] private Text HPtext;
    [SerializeField] private Text MPtext;
    [SerializeField] private Text SPtext;
    public GameObject pauseMenu;
    public GameObject debugMenu;

    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerHealth = playerController.GetComponent<PlayerHealth>();
        playerStamina = playerController.GetComponentInChildren<Stamina>();
        playerMana = playerController.GetComponentInChildren<PlayerMana>();
    }

    // Update is called once per frame
    void Update()
    {
        HPtext.text = playerHealth.HP.ToString();
        MPtext.text = playerMana.MP.ToString();
        SPtext.text = playerStamina.SP.ToString();
        if (GameController.IsPaused)
        {
            pauseMenu.SetActive(true);
        }
        else
        {
            pauseMenu.SetActive(false);
        }
    }
}
