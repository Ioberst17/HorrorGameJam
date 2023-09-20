using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboSystem : MonoBehaviour
{
    [SerializeField] private float comboInputTimeThreshold = 1f;
    [SerializeField] private float lastInputTime;
    [SerializeField] private int comboCount = 0;
    [SerializeField] private int maxComboCount = 3;
    PlayerAttackManager playerPrimaryWeapon;
    Dictionary<int, bool> hasExecuted = new Dictionary<int, bool>();
    // need to use this to iterate over and modify the dictionary
    List<int> comboNumbers = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        playerPrimaryWeapon = GetComponent<PlayerAttackManager>();
        hasExecuted.Add(1, false);
        hasExecuted.Add(2, false);
        hasExecuted.Add(3, false);
        comboNumbers = new List<int>(hasExecuted.Keys);
        EventSystem.current.playerCombo += UpdateTheHasExecutedDictionary;
    }

    private void OnDestroy()
    {
        EventSystem.current.playerCombo += UpdateTheHasExecutedDictionary;
    }

    // Check for input and track combocount
    void Update()
    {
        // Check for combo input
        if (Time.time - lastInputTime > comboInputTimeThreshold) 
        {
            // if too much time has passed reset combo
            ResetExecutionTracker();
        }
    }

    void UpdateTheHasExecutedDictionary(int comboNumber) { hasExecuted[comboNumber] = true; }

    private void ResetExecutionTracker()
    {
        comboCount = 0;
        foreach (var comboNumber in comboNumbers) { hasExecuted[comboNumber] = false; }
    }

    public void PerformCombo(int attackDirection)
    {
        // Increase combo count, if it is the initial hit
        // all other increments to combo count are through animation events
        if (comboCount == 0) { comboCount++; }

        // Perform different actions based on the combo count
        switch (comboCount)
        {
            case 1:
                // Perform first punch
                Debug.Log("First punch!");
                playerPrimaryWeapon.StartAttack(attackDirection, "PlayerBasicAttack1");
                comboCount++;
                break;
            case 2:
                // Perform second punch
                Debug.Log("Second punch!");
                if (hasExecuted[comboCount - 1])
                {
                    playerPrimaryWeapon.StartAttack(attackDirection, "PlayerBasicAttack2");
                    comboCount++;
                }
                break;
            case 3:
                // Perform third punch
                Debug.Log("Third punch!");
                if (hasExecuted[comboCount - 1])
                {
                    playerPrimaryWeapon.StartAttack(attackDirection, "PlayerBasicAttack3");
                    comboCount++;
                }
                break;
            case 4:
                // Perform third punch
                Debug.Log("Restart Combo!");
                ResetExecutionTracker();
                playerPrimaryWeapon.StartAttack(attackDirection, "PlayerBasicAttack1");
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
