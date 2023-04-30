using UnityEngine;
using TMPro;

public class UIHealthChangeDisplay : MonoBehaviour
{
    [SerializeField] private float xAdjustment;
    [SerializeField] private float yAdjustment;

    Vector2 adjustedPosition; // NOTE: Workaround, since numbers aren't spawning at player center

    public void ShowChange(float damage, string symbol)
    {

        // Instantiate the damage text prefab

        adjustedPosition = new Vector2(transform.position.x - xAdjustment, transform.position.y - yAdjustment); 

        GameObject damageTextInstance = Instantiate(Resources.Load("VFXPrefabs/TextDisplay") as GameObject, adjustedPosition, Quaternion.identity);
        TextMeshPro textMeshPro = damageTextInstance.GetComponent<TextMeshPro>();
        if (symbol == "Positive")
        {
            damageTextInstance.GetComponent<UICreateAndFadeText>().textColor = Color.green;
            textMeshPro.text = "+" + damage;
        }
        else if (symbol == "Negative")
        {
            damageTextInstance.GetComponent<UICreateAndFadeText>().textColor = Color.red;
            textMeshPro.text = "-" + damage;
        }
        else { Debug.LogFormat("UIHealthChangeDisplay.cs, attached to a gameobject named {0} is being given a value ({1}) it cannot process", gameObject.name, symbol); }

        UICreateAndFadeText damageTextScript = damageTextInstance.GetComponent<UICreateAndFadeText>();
        damageTextScript.moveSpeed = 1.0f;
        damageTextScript.fadeSpeed = 1.0f;
        damageTextScript.duration = 1.0f;
    }
}