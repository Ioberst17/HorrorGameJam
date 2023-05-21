using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpinSupport : MonoBehaviour
{
    public ParticleSystem.ShapeModule shape;

    [SerializeField] Vector3 speed;
    public float oscillationSpeed = 30f;
    public float minRotationX;
    public float maxRotationX;

    private void Start() { shape = GetComponent<ParticleSystem>().shape; }

    void Update()
    {
        transform.Rotate(speed * Time.deltaTime);
        RotateParticleSpaceOnXAxis();
    }

    private void RotateParticleSpaceOnXAxis()
    {
        float rotationX = Mathf.Lerp(minRotationX, maxRotationX, Mathf.PingPong(Time.time * oscillationSpeed, 1f));
        shape.rotation = new Vector3(rotationX, 0f, 0f);
    }
}
