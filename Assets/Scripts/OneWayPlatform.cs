using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private PlatformEffector2D effector;
    private OneWayPlatformPlayerDetection PlatformPlayerDetection;
    private GameController gameController;
    public float waitTime;
    public bool PlayerInZone;
    // Start is called before the first frame update
    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
        PlatformPlayerDetection = GetComponentInChildren<OneWayPlatformPlayerDetection>();
        gameController = FindObjectOfType<GameController>();
        PlayerInZone = false;
        effector.useColliderMask = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerInZone && gameController.YInput > 0)
        {
            waitTime = 0.5f;
        }
        if (PlayerInZone && gameController.YInput < 0)
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
        if (gameController.YInput > 0)
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
