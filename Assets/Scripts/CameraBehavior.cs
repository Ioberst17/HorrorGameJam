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
        transform.position = new Vector3(player.position.x, player.position.y, -10);
        Holdcounter = 0;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        AdjustCameraDown();

        transform.position = new Vector3(cameraFocus.position.x, cameraFocus.position.y, -10);
    }

    private void AdjustCameraDown()
    {
        if (Input.GetKey("down") || Input.GetKey(KeyCode.S))
        {
            Holdcounter += 0.01f;
            if (Holdcounter > 1)
            {
                if (cameraFocus.localPosition.y > -3)
                {
                    cameraFocus.localPosition = new Vector3(cameraFocus.localPosition.x, cameraFocus.localPosition.y - 0.01f, 0);
                    //Debug.Log("should go down " + cameraFocus.localPosition.y);
                }
            }
        }
        else
        {
            Holdcounter = 0;
            if (cameraFocus.localPosition.y < 0)
            {
                cameraFocus.position = new Vector3(cameraFocus.position.x, cameraFocus.position.y + 0.01f, 0);
                //Debug.Log("should go up " + cameraFocus.localPosition.y);
            }
        }
    }
}
