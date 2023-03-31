using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Shield))]
public class Parry : MonoBehaviour
{
    // These variables control the timing and duration of the parry window.
    public float parryWindowDuration = 1.2f; // In seconds.
    public float parryCooldown = 1f; // In seconds.
    private bool canParry = true;
    public bool isParrying = false;
    private float lastParryTime = -Mathf.Infinity;

    // This function is called when the player presses the parry button.
    public void Execute()
    {
        if (canParry && Time.time - lastParryTime > parryCooldown)
        {
            // Set the parry window to be active.
            canParry = false; isParrying = true;
            lastParryTime = Time.time;
            StartCoroutine(ParryWindow());
        }
    }

    // This coroutine represents the parry window, during which the player can deflect attacks.
    private IEnumerator ParryWindow()
    {
        float startTime = Time.time;

        while (parryWindowDuration > Time.time - startTime)
        {
            // During the parry window, check for collisions with enemy attacks.
            yield return null;
        }

        // The parry window is over. Reset the canParry flag.
        canParry = true; isParrying = false;
    }

    // This function is called when the player collides with an enemy attack.
    public bool GetParryStatus() { return isParrying; }
}
