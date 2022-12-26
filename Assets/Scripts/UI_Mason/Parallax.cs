using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private Vector3 startPos;
    private float repeatWidth;
    public float parallaxSpeed = 5;
    private float leftBound = -15; // used as boundary to destroy background objects when they are too far left

    private void Start()
    {
        startPos = transform.position; // Establish the default starting position 
        repeatWidth = GetComponent<BoxCollider2D>().size.x / 2; // Set repeat width to half of the background
    }

    private void Update()
    {
        transform.Translate(Vector3.left * Time.deltaTime * parallaxSpeed, Space.World); // moves object forward

        // If background moves left by its repeat width, move it back to start position
        if (transform.position.x < startPos.x - repeatWidth)
        {
            transform.position = startPos;
        }
    }


}