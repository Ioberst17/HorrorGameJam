using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResetAllBindings : MonoBehaviour
{
    private GameController gameController;

    [SerializeField]
    private InputActionAsset inputActions;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    public void ResetBindings()
    {
        foreach(InputActionMap map in inputActions.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }
        PlayerPrefs.DeleteKey("rebinds");
    }

    public void ResetControlSchemeBindings()
    {
        foreach (InputActionMap map in inputActions.actionMaps)
        {
            foreach(InputAction action in map.actions)
            {
                action.RemoveBindingOverride(InputBinding.MaskByGroup(gameController.CurrentControlScheme));
            }
        }
        PlayerPrefs.DeleteKey("rebinds");
    }
}
