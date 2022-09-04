using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//IMPORTANT - not currently using this script. Doesnt work with what i have. DO NOT IMPLEMENT.
public class DialogueBoxHandler_Mason : MonoBehaviour
{

    [SerializeField] private GameObject dialogueBox;
    [SerializeField] Animator dialogueBoxAnimation;
    [SerializeField] TMP_Text dialogueText;

    public void DialogueBoxPopUp(string text)
    {
        dialogueBox.SetActive(true);
        dialogueText.text = text;
        dialogueBoxAnimation.SetTrigger("pop");
    }
}
