using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Transform player;
    public float Hleeway;
    public float Vleeway;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(player.position.x, player.position.y, -5);
    }

    // Update is called once per frame
    void Update()
    {
        if (((transform.position.x) - (player.position.x)) > Hleeway)
        {
            transform.position = new Vector3(transform.position.x - ((transform.position.x - player.position.x) - Hleeway), transform.position.y, -5);
        }
        if (((transform.position.x) - (player.position.x)) < -Hleeway)
        {
            transform.position = new Vector3(transform.position.x - ((transform.position.x - player.position.x) + Hleeway), transform.position.y, -5);
        }
        if (((transform.position.y) - (player.position.y)) > Vleeway)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - ((transform.position.y - player.position.y) - Vleeway), -5);
        }
        if (((transform.position.y) - (player.position.y)) < -Vleeway)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - ((transform.position.y - player.position.y) + Vleeway), -5);
        }
        //transform.position = new Vector3(player.position.x, player.position.y, -5);
    }
}
