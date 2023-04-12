using UnityEngine;

public class VFXDustCloud : MonoBehaviour
{
    [SerializeField] private ParticleSystem dustParticles;   // Reference to the dust particle system

    private void Start()
    {
        CreateDustCloud();
    }

    private void CreateDustCloud()
    { 
        // Enable the particle system and play the explosion
        dustParticles.Play();
        Destroy(gameObject, 3f);
    }
}