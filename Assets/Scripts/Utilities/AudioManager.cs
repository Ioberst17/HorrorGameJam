using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public Sound[] themes;

    // Start is called before the first frame update
    void Awake()
    {
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        foreach (Sound t in themes)
        {
            t.source = gameObject.AddComponent<AudioSource>();
            t.source.clip = t.clip;

            t.source.volume = t.volume;
            t.source.pitch = t.pitch;
            t.source.loop = t.loop;
        }

    }

    private void Start() // handles theme music
    {
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            PlayTheme("Title Theme");
        }
        else
        {
            string themeToPlay = "Scene " + SceneManager.GetActiveScene().buildIndex.ToString() + " Theme";
            PlayTheme(themeToPlay);
        }
    }

    public void PlaySFX (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null)
        {
            Debug.LogWarning("Sound " + name + " doesn't exist - check for in AudioManager");
            return;
        }
        else
        {
            s.source.Play();
        }
    }

    public void PlayTheme(string name)
    {
        Sound t = Array.Find(themes, theme => theme.name == name);
        if (t == null)
        {
            Debug.LogWarning("Theme " + name + " doesn't exist - check for in AudioManager");
            return;
        }
        t.source.Play();
    }
}
