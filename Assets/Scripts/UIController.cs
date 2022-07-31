using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private PlayerController PlayerController;
    [SerializeField]
    private Text HPtext;
    [SerializeField]
    private Text MPtext;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HPtext.text = PlayerController.HP.ToString();
        MPtext.text = PlayerController.MP.ToString();
    }
}
