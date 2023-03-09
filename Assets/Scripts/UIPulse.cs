using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))] // image to pulse
public class UIPulse : MonoBehaviour
{
    public bool pulseTrigger;
    public bool continuousPulse;
    [SerializeField] private float sizeShift = 0.02f;
    [SerializeField] private float stepCounter = 0.2f;
    [SerializeField] private float pulseWaitTime = 0.05f;

    // Start is called before the first frame update
    void Start() { pulseTrigger = false; }

    // Update is called once per frame
    void Update() { if (continuousPulse || pulseTrigger) { StartCoroutine("Pulsing"); } }

    private IEnumerator Pulsing()
    {
        pulseTrigger = false;

        for (float i = 0f; i < 1f; i += stepCounter) // grow
        {
            transform.localScale = new Vector3(
                Mathf.Lerp(transform.localScale.x, transform.localScale.x + sizeShift, Mathf.SmoothStep(0f, 1f, i)),
                Mathf.Lerp(transform.localScale.y, transform.localScale.y + sizeShift, Mathf.SmoothStep(0f, 1f, i)),
                Mathf.Lerp(transform.localScale.z, transform.localScale.z + sizeShift, Mathf.SmoothStep(0f, 1f, i))
                );
        yield return new WaitForSeconds(pulseWaitTime);
        }

        for (float i = 0f; i < 1f; i += stepCounter) // shrink
        {
            transform.localScale = new Vector3(
                Mathf.Lerp(transform.localScale.x, transform.localScale.x - sizeShift, Mathf.SmoothStep(0f, 1f, i)),
                Mathf.Lerp(transform.localScale.y, transform.localScale.y - sizeShift, Mathf.SmoothStep(0f, 1f, i)),
                Mathf.Lerp(transform.localScale.z, transform.localScale.z - sizeShift, Mathf.SmoothStep(0f, 1f, i))
                );
            yield return new WaitForSeconds(pulseWaitTime);
        }
    }
}
