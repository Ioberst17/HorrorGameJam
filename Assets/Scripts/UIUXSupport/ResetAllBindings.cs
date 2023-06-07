using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResetAllBindings : MonoBehaviour
{
    private GameController gameController;

    [SerializeField]
    private PlayerInput playerInput;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
        playerInput = FindObjectOfType<PlayerInput>();
    }

    public void ResetBindings()
    {
        foreach(InputActionMap map in playerInput.actions.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }
        PlayerPrefs.DeleteKey("rebinds");
    }

    public void ResetControlSchemeBindings()
    {
        foreach (InputActionMap map in playerInput.actions.actionMaps)
        {
            foreach(InputAction action in map.actions)
            {
                action.RemoveBindingOverride(InputBinding.MaskByGroup(gameController.CurrentControlScheme));
            }
        }
        PlayerPrefs.DeleteKey("rebinds");
    }
}
