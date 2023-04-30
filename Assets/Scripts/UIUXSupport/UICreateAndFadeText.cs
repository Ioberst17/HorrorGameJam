using System.Collections;
using UnityEngine;
using TMPro;

public class UICreateAndFadeText : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public float fadeSpeed = 1.0f;
    public float duration = 1.0f;
    public float oscillationSpeed = 2.0f;
    public float oscillationAmplitude = 0.5f;

    private TextMeshPro textMeshPro;
    public Color textColor;
    private float elapsedTime;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshPro>();
        textColor = textMeshPro.color;
        elapsedTime = 0;
    }

    private void Update()
    {
        float delta = Time.deltaTime;
        elapsedTime += delta;

        // Move the text upwards
        transform.position += Vector3.up * moveSpeed * delta;

        // Oscillate the text left and right
        float oscillation = Mathf.Sin(elapsedTime * oscillationSpeed) * oscillationAmplitude;
        transform.position += Vector3.right * oscillation * delta;

        // Fade out the text
        textColor.a -= fadeSpeed * delta;
        textMeshPro.color = textColor;

        // Destroy the text after duration
        duration -= delta;
        if (duration <= 0)
        {
            Destroy(gameObject);
        }
    }
}
