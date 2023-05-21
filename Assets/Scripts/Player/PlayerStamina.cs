using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    public float SP { get; set; }
    public float maxSP { get; set; }

    private void Start()
    {
        maxSP = 100;
        SP = maxSP;
    }
    public void ChangeSP(int healthToAdd)
    {
        if ((SP += healthToAdd) < maxSP) { SP += healthToAdd; }
        else { SP = maxSP; }
    }
}
