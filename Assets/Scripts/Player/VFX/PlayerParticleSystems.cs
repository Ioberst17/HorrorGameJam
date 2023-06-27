using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticleSystems : MonoBehaviour // GAME OBJECT THAT STORES AND PLAYS PARTICLE EFFECTS
{
    public List<GameObject> particleEffects;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform) // create a list of game objects with particle systems from child objects
        {
            if (child.GetComponent<ParticleSystem>() != null)
            {
                particleEffects.Add(child.gameObject);
            }
        }
    }

    public void PlayEffect(string name)
    {
        GameObject particleEffect = particleEffects.Find(x => x.name.Contains(name));
        if (particleEffect == null)
        {
            Debug.LogWarning("Particle Effect " + name + " is not in 'Visual Effects' game Object - check to see if it loaded at runtime in " +
                "the game object in the Player Prefab");
            return;
        }
        else { particleEffect.GetComponent<ParticleSystem>().Play(); }
    }
}
