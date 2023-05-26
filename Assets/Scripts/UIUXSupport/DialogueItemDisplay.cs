using Coffee.UIExtensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueItemDisplay : MonoBehaviour
{
    [SerializeField] private Image objectToDisplay;
    [SerializeField] private UIParticle particles;
    [SerializeField] private TextMeshProUGUI textToDisplay;

    // Start is called before the first frame update
    void Start()
    {
        objectToDisplay = GetComponentInChildren<Image>(true);
        objectToDisplay.gameObject.SetActive(false);

        particles = GetComponentInChildren<UIParticle>(true);
        particles.Stop();

        textToDisplay = GetComponentInChildren<TextMeshProUGUI>();
        textToDisplay.gameObject.SetActive(false);
    }

    public void Display(bool displayStatus, Sprite spriteToDisplay = null, string textToDisplay = "")
    {
        if (displayStatus)
        {
            if (objectToDisplay != null) 
            {
                objectToDisplay.gameObject.SetActive(true);
                objectToDisplay.sprite = spriteToDisplay;

            }
            if (particles != null)
            {
                particles.gameObject.SetActive(true);
                particles.Play();
            }
            if (this.textToDisplay != null) { if (this.textToDisplay.text != "") 
                {
                    this.textToDisplay.gameObject.SetActive(true);
                    this.textToDisplay.text = textToDisplay;
                } 
            }
        }
        else
        {
            if (objectToDisplay != null)
            {
                objectToDisplay.gameObject.SetActive(false);
                objectToDisplay.sprite = null;
            }
            if (particles != null)
            {
                particles.Stop();
                particles.gameObject.SetActive(false);
            }
            if (this.textToDisplay != null) { { this.textToDisplay.text = ""; } }
        }
    }
}
