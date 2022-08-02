using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private GameController GameController;
    [SerializeField]
    private Text HPtext;
    [SerializeField]
    private Text MPtext;
    public GameObject pauseMenu;
    public GameObject debugMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HPtext.text = GameController.GetHP().ToString();
        MPtext.text = GameController.GetMP().ToString();
        if (GameController.isPaused)
        {
            pauseMenu.SetActive(true);
        }
        else
        {
            pauseMenu.SetActive(false);
        }
    }
}
