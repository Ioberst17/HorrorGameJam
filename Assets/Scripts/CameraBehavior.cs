using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform cameraFocus;
    private GameController gameController;
    private PlayerController playerController;

    [SerializeField] private float shakeDuration = 0f;
    [SerializeField] private float shakeMagnitude = 0.7f;
    [SerializeField] private float dampingSpeed = 1.0f;
    private Vector3 initialPosition;

    private float Holdcounter;
    public float Hleeway;
    public float Vleeway;

    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        playerController = FindObjectOfType<PlayerController>();
        transform.position = new Vector3(player.position.x, player.position.y, -10);
        Holdcounter = 0;

        // Store the initial position of the camera focus
        initialPosition = cameraFocus.localPosition;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(cameraFocus.position.x, cameraFocus.position.y, -10);

        if (shakeDuration > 0)
        {
            cameraFocus.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;

            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            cameraFocus.localPosition = initialPosition;
        }
    }

    public void AdjustCameraDown()
    {
        if (gameController.YInput < 0 && playerController.IsGrounded)
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
        Holdcounter = 0;
        if (cameraFocus.localPosition.y < 0)
        {
            cameraFocus.position = new Vector3(cameraFocus.position.x, cameraFocus.position.y + 0.01f, 0);
            //Debug.Log("should go up " + cameraFocus.localPosition.y);
        }
    }

    public void ShakeScreen(float duration)
    {
        shakeDuration = duration;
    }
}
