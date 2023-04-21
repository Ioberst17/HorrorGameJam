using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lucidity_UI : MonoBehaviour
{

    [SerializeField] private Lucidity lucidity;

    [SerializeField] private Image lucidityHourGlass;
    public float lucidityTime;

    // Start is called before the first frame update
    void Start()
    {
        lucidityTime = lucidity.level;
        lucidityHourGlass.fillAmount = lucidityTime / 100f;
    }

    // Update is called once per frame
    void Update()
    {
        lucidityTime = lucidity.level;
        lucidityHourGlass.fillAmount = lucidityTime / 100f;
    }


}
