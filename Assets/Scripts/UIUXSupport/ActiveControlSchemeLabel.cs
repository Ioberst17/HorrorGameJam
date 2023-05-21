using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ActiveControlSchemeLabel : MonoBehaviour
{
    private Text controlSchemeText;
    private PlayerInput playerInput; // assigned in inspector

    private void Start() { playerInput = FindObjectOfType<PlayerInput>(); controlSchemeText = GetComponent<Text>(); }

    private void Update() { controlSchemeText.text = playerInput.currentControlScheme; }
}
