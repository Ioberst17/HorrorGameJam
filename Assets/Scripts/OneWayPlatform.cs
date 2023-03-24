using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private PlatformEffector2D effector;
    private OneWayPlatformPlayerDetection PlatformPlayerDetection;
    public float waitTime;
    public bool PlayerInZone;
    // Start is called before the first frame update
    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
        PlatformPlayerDetection = GetComponentInChildren<OneWayPlatformPlayerDetection>();
        PlayerInZone = false;
        effector.useColliderMask = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerInZone && (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S)))
        {
            waitTime = 0.5f;
        }
        if (PlayerInZone && (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)))
        {
            if (waitTime <= 0)
            {
                effector.useColliderMask = true;
                waitTime = 0.05f;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)|| Input.GetButtonDown("Jump") || Input.GetKeyDown("up"))
        {
            effector.useColliderMask = false;
        }
        if (!PlayerInZone)
        {
            effector.useColliderMask = false;
            waitTime = 0f;
        }
    }
}
