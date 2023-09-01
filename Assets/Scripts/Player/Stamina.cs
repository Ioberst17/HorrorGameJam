using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    public float SP { get; set; }
    public float maxSP { get; set; }

    private void Start()
    {
        maxSP = 100;
        SP = maxSP;
    }
    public void ChangeSP(int spToAdd)
    {
        if ((SP += spToAdd) < maxSP) { SP += spToAdd; }
        else { SP = maxSP; }
    }
}
