using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))] // image to pulse
public class UIPulse : MonoBehaviour
{
    public bool pulseTrigger;
    public bool continuousPulse;
    [SerializeField] private float sizeShift = 0.02f;
    [SerializeField] private float pulseSpeed = 1f;
    private Vector3 originalScale; // the original scale of the object
    private Vector3 targetScale;
    private float pulseTime; // the current pulse time, in seconds
    private bool isPulsing;

    private IEnumerator pulseCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        pulseTrigger = false;
        originalScale = transform.localScale;
        pulseTime = 0f;
        isPulsing = false;

        if (continuousPulse)
        {
            pulseCoroutine = Pulsing();
            StartCoroutine(pulseCoroutine);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pulseTrigger)
        {
            if (pulseCoroutine != null)
            {
                StopCoroutine(pulseCoroutine);
                pulseCoroutine = null;
            }

            pulseCoroutine = Pulsing();
            StartCoroutine(pulseCoroutine);
            pulseTrigger = false;
        }
    }

    private IEnumerator Pulsing()
    {
        isPulsing = true;

        // grow
        targetScale = originalScale + new Vector3(sizeShift, sizeShift, sizeShift);
        pulseTime = 0f;
        while (pulseTime < 1f)
        {
            pulseTime += Time.deltaTime * pulseSpeed;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, pulseTime);
            yield return null;
        }

        // shrink to normal
        targetScale = originalScale;
        pulseTime = 0f;
        while (pulseTime < 1f)
        {
            pulseTime += Time.deltaTime * pulseSpeed;
            transform.localScale = Vector3.Lerp(originalScale + new Vector3(sizeShift, sizeShift, sizeShift), targetScale, pulseTime);
            yield return null;
        }

        transform.localScale = originalScale;

        isPulsing = false;
        if (continuousPulse)
        {
            pulseCoroutine = Pulsing();
            StartCoroutine(pulseCoroutine);
        }
    }

    private void OnDestroy()
    {
        if (isPulsing && pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
        }
    }
}
