using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMana : MonoBehaviour
{
    public int MP { get; set; }
    public int maxMP { get; set; }

    private void Start()
    {
        maxMP = 100;
        MP = maxMP;
    }
    public void ChangeMP(int healthToAdd)
    {
        if ((MP += healthToAdd) < maxMP) { MP += healthToAdd; }
        else { MP = maxMP; }
    }
}
