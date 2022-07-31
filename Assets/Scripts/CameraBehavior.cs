using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    [SerializeField]
    private Transform player;
    [SerializeField]
    private Transform cameraFocus;
    private float Holdcounter;
    public float Hleeway;
    public float Vleeway;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(player.position.x, player.position.y, -5);
        Holdcounter = 0;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        AdjustCameraDown();

        if (((transform.position.x) - (cameraFocus.position.x)) > Hleeway)
        {
            transform.position = new Vector3(transform.position.x - ((transform.position.x - cameraFocus.position.x) - Hleeway), transform.position.y, -5);
        }
        if (((transform.position.x) - (cameraFocus.position.x)) < -Hleeway)
        {
            transform.position = new Vector3(transform.position.x - ((transform.position.x - cameraFocus.position.x) + Hleeway), transform.position.y, -5);
        }
        if (((transform.position.y) - (cameraFocus.position.y)) > Vleeway)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - ((transform.position.y - cameraFocus.position.y) - Vleeway), -5);
        }
        if (((transform.position.y) - (cameraFocus.position.y)) < 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - ((transform.position.y - cameraFocus.position.y) + Vleeway), -5);
        }
        //transform.position = new Vector3(player.position.x, player.position.y, -5);
    }

    private void AdjustCameraDown()
    {
        if (Input.GetKey("down") || Input.GetKey(KeyCode.S))
        {
            Holdcounter += 0.01f;
            if (Holdcounter > 1)
            {
                if (transform.position.y + cameraFocus.position.y > -10)
                {
                    cameraFocus.position = new Vector3(cameraFocus.position.x, cameraFocus.position.y - 0.01f, 0);
                    Debug.Log("should go down " + (transform.position.y - cameraFocus.position.y));
                }
            }
        }
        else
        {
            Holdcounter = 0;
            if (transform.position.y + cameraFocus.position.y < 0)
            {
                cameraFocus.position = new Vector3(cameraFocus.position.x, cameraFocus.position.y + 0.01f, 0);
                Debug.Log("should go up " + (transform.position.y - cameraFocus.position.y));
            }
        }
    }
}
