using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboSystem : MonoBehaviour
{
    [SerializeField] private float comboInputTimeThreshold = 1f;
    [SerializeField] private float lastInputTime;
    [SerializeField] private int comboCount = 0;
    [SerializeField] private int maxComboCount = 3;
    PlayerPrimaryWeapon playerPrimaryWeapon;

    // Start is called before the first frame update
    void Start()
    {
        playerPrimaryWeapon = GetComponent<PlayerPrimaryWeapon>();
    }

    // Check for input and track combocount
    void Update()
    {
        // Check for combo input
        if (Time.time - lastInputTime > comboInputTimeThreshold) { comboCount = 0; }
    }

    public void PerformCombo(int attackDirection)
    {
        // Increase combo count
        comboCount++;

        // Perform different actions based on the combo count
        switch (comboCount)
        {
            case 1:
                // Perform first punch
                Debug.Log("First punch!");
                StartCoroutine(playerPrimaryWeapon.AttackActiveFrames(attackDirection, "PlayerBasicAttack1"));
                break;
            case 2:
                // Perform second punch
                Debug.Log("Second punch!");
                StartCoroutine(playerPrimaryWeapon.AttackActiveFrames(attackDirection, "PlayerBasicAttack2"));
                break;
            case 3:
                // Perform third punch
                Debug.Log("Third punch!");
                StartCoroutine(playerPrimaryWeapon.AttackActiveFrames(attackDirection, "PlayerBasicAttack3"));
                //StartCoroutine(playerPrimaryWeapon.AttackActiveFrames(attackDirection, "PlayerThirdPunch"));
                break;
            case 4:
                // Perform third punch
                Debug.Log("Restart Combo!");
                comboCount = 0;
                StartCoroutine(playerPrimaryWeapon.AttackActiveFrames(attackDirection, "PlayerBasicAttack1"));
                //StartCoroutine(playerPrimaryWeapon.AttackActiveFrames(attackDirection, "PlayerThirdPunch"));
                break;
        }

        // Update the last input time
        lastInputTime = Time.time;
    }

    public void PerformDirectionalCombo(Vector2 inputDirection)
    {
        if (comboCount == maxComboCount)
        {
            // Check if the input direction is up or straight (based on your game's coordinate system)
            if (inputDirection == Vector2.up)
            {
                Debug.Log("Final punch - Up direction!");
            }
            else if (Mathf.Approximately(inputDirection.x, 0f) && Mathf.Approximately(inputDirection.y, -1f))
            {
                Debug.Log("Final punch - Straight direction!");
            }
            else
            {
                Debug.Log("Invalid final punch direction!");
            }

            // Reset combo count
            comboCount = 0;
        }

        // Update the last input time
        lastInputTime = Time.time;
    }
}
