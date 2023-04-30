using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UIOscillate : MonoBehaviour
{

    Component renderedImageToMove;
    /*[Range(.01f, 10)]*/ public float oscillationSpeed = 2.0f;
    [Tooltip("Set between 0 and 2 for in-game objects, 500 to 1000 for UI objects")] public float oscillationAmplitude = 0.5f;
    public bool shouldLoop = true;
    public enum floatDirection { Vertical, Horizontal, Both };
    public floatDirection direction = floatDirection.Vertical;
    private float oscillation;

    //Loop specific internal
    private float elapsedTimeLoop;
    private float timeToAddLoop;

    //Trigger specific
    private float timeToAddTrigger;
    public float timeToPlayTrigger;
    public float elapsedTimeTrigger;
    public bool hasBeenTriggered;
    private bool useAnchoredPosition;
    private Vector2 renderedImageOriginalPosition;


    // Start is called before the first frame update
    void Start()
    {
        renderedImageToMove = GetComponent<SpriteRenderer>();
        if (renderedImageToMove == null) { renderedImageToMove = GetComponent<Image>(); }


        if (useAnchoredPosition)
        {
            if (renderedImageToMove is Image) { { renderedImageOriginalPosition = GetComponent<RectTransform>().anchoredPosition; } }
            else if (renderedImageToMove is SpriteRenderer) { renderedImageOriginalPosition = renderedImageToMove.transform.localPosition; }
        }
        else
        {
            if (renderedImageToMove is Image) { renderedImageOriginalPosition = GetComponent<RectTransform>().position; }
            else if (renderedImageToMove is SpriteRenderer) { renderedImageOriginalPosition = renderedImageToMove.transform.position; }
        }
        //Debug.Log("Rendered image's original position is reported as " + renderedImageOriginalPosition);
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldLoop) { FloatLoop(direction); }
        if (hasBeenTriggered) { FloatTrigger(direction); }
    }

    public void FloatLoop(floatDirection direction)
    {
        timeToAddLoop = Time.deltaTime; elapsedTimeLoop += timeToAddLoop;

        OscillationLogic(elapsedTimeLoop, timeToAddLoop);
    }

    private void FloatTrigger(floatDirection direction)
    {
        timeToAddTrigger = Time.deltaTime;
        elapsedTimeTrigger += timeToAddTrigger;
        if (elapsedTimeTrigger < timeToPlayTrigger) { OscillationLogic(elapsedTimeTrigger, timeToAddTrigger); }
        else { ResetTrigger(); }
    }

    void OscillationLogic(float elapsedTime, float timeToAdd)
    {
        oscillation = Mathf.Sin(elapsedTime * oscillationSpeed) * oscillationAmplitude;
        if (useAnchoredPosition)
        {
            if (renderedImageToMove is Image)
            {
                if (direction == floatDirection.Vertical) { GetComponent<RectTransform>().anchoredPosition += Vector2.up * oscillation * timeToAdd; }
                else if (direction == floatDirection.Horizontal) { GetComponent<RectTransform>().anchoredPosition += Vector2.right * oscillation * timeToAdd; }
            }
            else if (renderedImageToMove is SpriteRenderer)
            {
                if (direction == floatDirection.Vertical) { transform.localPosition += Vector3.up * oscillation * timeToAdd; }
                else if (direction == floatDirection.Horizontal) { transform.localPosition += Vector3.right * oscillation * timeToAdd; }
            }
        }
        else
        {
            if (renderedImageToMove is Image)
            {
                if (direction == floatDirection.Vertical) { GetComponent<RectTransform>().position += Vector3.up * oscillation * timeToAdd; }
                else if (direction == floatDirection.Horizontal) { GetComponent<RectTransform>().position += Vector3.right * oscillation * timeToAdd; }
            }
            else if (renderedImageToMove is SpriteRenderer)
            {
                if (direction == floatDirection.Vertical) { transform.position += Vector3.up * oscillation * timeToAdd; }
                else if (direction == floatDirection.Horizontal) { transform.position += Vector3.right * oscillation * timeToAdd; }
            }
        }


    }

    void ResetTrigger()
    {
        hasBeenTriggered = false; elapsedTimeTrigger = 0;
        if (useAnchoredPosition)
        {
            if (renderedImageToMove is Image) { GetComponent<RectTransform>().anchoredPosition = renderedImageOriginalPosition; }
            else if (renderedImageToMove is SpriteRenderer) { transform.localPosition = renderedImageOriginalPosition; }
        }
        else
        {
            if (renderedImageToMove is Image) { GetComponent<RectTransform>().position = renderedImageOriginalPosition; }
            else if (renderedImageToMove is SpriteRenderer) { transform.position = renderedImageOriginalPosition; }
        }
    }
}
