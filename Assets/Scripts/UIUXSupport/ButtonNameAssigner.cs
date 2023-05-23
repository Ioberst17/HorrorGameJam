using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonNameAssigner : MonoBehaviour
{
    public enum TextStyle { Text, TextMeshProUGUI }
    public TextStyle textStyle;
    TextMeshProUGUI buttonTextMeshProUGUI;
    Text buttonText;


    private void Start()
    {
        Button button = GetComponentInChildren<Button>();

        if (button != null)
        {
            if (textStyle == TextStyle.TextMeshProUGUI) { buttonTextMeshProUGUI = button.GetComponentInChildren<TextMeshProUGUI>(); }
            else if(textStyle == TextStyle.Text) { buttonText = button.GetComponentInChildren<Text>(); }

            if (buttonTextMeshProUGUI != null) { buttonTextMeshProUGUI.text = gameObject.name; }
            else if (buttonText.text != "") { buttonText.text = gameObject.name; }
        }
        else
        {
            Debug.LogError("Button component not found!");
        }
    }
}
