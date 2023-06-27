using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualEffectsController : MonoBehaviour
{
    public List<ParticleSystem> particleSystems = new List<ParticleSystem>();

    private void Start()
    {
        FillParticleSystemList();
    }

    private void FillParticleSystemList()
    {
        particleSystems.Clear();
        ParticleSystem[] childParticleSystems = GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem ps in childParticleSystems)
        {
            if (ps.transform.parent == transform)
            {
                particleSystems.Add(ps);
            }
        }
    }

    public void PlayParticleSystem(string particleSystemName)
    {
        ParticleSystem ps = particleSystems.Find(p => p.name == particleSystemName);
        if (ps != null)
        {
            ps.Play();
        }
        else
        {
            Debug.LogWarning("Particle system with the name " + particleSystemName + " not found.");
        }
    }

    public void LoopParticleSystem(string particleSystemName, bool loop)
    {
        ParticleSystem ps = particleSystems.Find(p => p.name == particleSystemName);
        if (ps != null)
        {
            var mainModule = ps.main;
            mainModule.loop = loop;
        }
        else
        {
            Debug.LogWarning("Particle system with the name " + particleSystemName + " not found.");
        }
    }

    public void StopParticleSystem(string particleSystemName)
    {
        ParticleSystem ps = particleSystems.Find(p => p.name == particleSystemName);
        if (ps != null)
        {
            ps.Stop();
        }
        else
        {
            Debug.LogWarning("Particle system with the name " + particleSystemName + " not found.");
        }
    }
}
