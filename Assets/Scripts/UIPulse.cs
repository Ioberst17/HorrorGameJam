using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))] // image to pulse
public class UIPulse : MonoBehaviour
{
    private bool pulsingAllowed;
    [SerializeField] private float sizeShift = 0.02f;
    [SerializeField] private float stepCounter = 0.2f;

    // Start is called before the first frame update
    void Start() { pulsingAllowed = true; }

    // Update is called once per frame
    void Update() { if (pulsingAllowed) { StartCoroutine("Pulsing"); } }

    private IEnumerator Pulsing()
    {
        pulsingAllowed = false;

        for (float i = 0f; i < 1f; i += stepCounter) // shrink
        {
            transform.localScale = new Vector3(
                Mathf.Lerp(transform.localScale.x, transform.localScale.x + sizeShift, Mathf.SmoothStep(0f, 1f, i)),
                Mathf.Lerp(transform.localScale.y, transform.localScale.y + sizeShift, Mathf.SmoothStep(0f, 1f, i)),
                Mathf.Lerp(transform.localScale.z, transform.localScale.z + sizeShift, Mathf.SmoothStep(0f, 1f, i))
                );
        yield return new WaitForSeconds(0.005f);
        }

        for (float i = 0f; i < 1f; i += stepCounter) // shrink
        {
            transform.localScale = new Vector3(
                Mathf.Lerp(transform.localScale.x, transform.localScale.x - sizeShift, Mathf.SmoothStep(0f, 1f, i)),
                Mathf.Lerp(transform.localScale.y, transform.localScale.y - sizeShift, Mathf.SmoothStep(0f, 1f, i)),
                Mathf.Lerp(transform.localScale.z, transform.localScale.z - sizeShift, Mathf.SmoothStep(0f, 1f, i))
                );
            yield return new WaitForSeconds(0.005f);
        }

        pulsingAllowed = true;
    }
}
