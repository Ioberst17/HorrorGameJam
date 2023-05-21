using System.Collections;
using UnityEngine;

public class ScreenFade : MonoBehaviour
{
    public float fadeDuration = 1f;
    public Color fadeColor = Color.black;
    public int drawDepth = -1000;

    private float alpha = 0f;
    private int fadeDirection = 1;
    private bool isFading = false;

    private void OnGUI()
    {
        if (isFading)
        {
            alpha += fadeDirection * (1f / fadeDuration) * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);

            GUI.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            GUI.depth = drawDepth;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        }
    }

    // Call this method to fade the screen to black
    public void FadeToBlack()
    {
        StartFade(1);
    }

    // Call this method to fade the screen from black
    public void FadeFromBlack()
    {
        StartFade(-1);
    }

    // Coroutine to handle the fade effect
    private void StartFade(int direction)
    {
        fadeDirection = direction;
        isFading = true;
        StartCoroutine(WaitForFadeComplete());
    }

    private IEnumerator WaitForFadeComplete()
    {
        yield return new WaitForSeconds(fadeDuration);
        isFading = false;
    }
}
