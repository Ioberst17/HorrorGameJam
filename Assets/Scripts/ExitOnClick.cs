using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitOnClick : MonoBehaviour
{
    public void ExitGame()
    {
        Debug.Log("Quitting the Game");
        Application.Quit();
    }
}