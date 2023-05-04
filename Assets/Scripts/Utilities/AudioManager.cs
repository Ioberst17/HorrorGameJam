using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public Sound[] themes;
    public Sound currentTheme;

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
        //float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        //SetSFXVolume(sfxVolume);
        

        if (SceneManager.GetActiveScene().buildIndex == 0) { PlayTheme("Title Theme"); SetThemeVolume(); }
        else if (SceneManager.GetActiveScene().name == "CombatMode") { PlayTheme("Scene 2 Theme"); SetThemeVolume(); }
        else
        {
            string themeToPlay = "Scene " + SceneManager.GetActiveScene().buildIndex.ToString() + " Theme";
            PlayTheme(themeToPlay);
            SetThemeVolume();
        }
    }

    public Sound GetSFX(string name) 
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) { Debug.LogWarning("AudioManager: sound " + name + " not found. Check for in AudioManager"); }
        return s;
    }

    public void PlaySFX (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        CheckForError(s);
    }

    public void LoopSFX(string name, bool loopState)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " doesn't exist - check for in AudioManager");
            return;
        }
        else
        {
            if (loopState == true) { if (!s.source.isPlaying) { s.loop = true; s.source.Play(); } }
            else { s.source.Stop(); }
        }
    }

    public void PlayTheme(string name)
    {
        currentTheme = Array.Find(themes, theme => theme.name == name);
        CheckForError(currentTheme);
    }

    public void UpdateThemePitch(float pitch)
    {
        currentTheme.source.pitch = pitch;
    }

    private void CheckForError(Sound sound)
    {
        if (sound == null)
        {
            Debug.LogWarning(sound + " couldn't be found - check for in AudioManager");
            return;
        }
        else { sound.source.Play(); }
    }

    public void SetSFXVolume(float volume)
    {
        foreach (Sound s in sounds)
        {
            s.source.volume = volume;
        }

        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void SetThemeVolume(float volume)
    {
        foreach (Sound t in themes) { t.source.volume = volume; }

        currentTheme.source.volume = volume;

        PlayerPrefs.SetFloat("ThemeVolume", volume);
    }

    public void SetThemeVolume()
    {
        float themeVolume = PlayerPrefs.GetFloat("ThemeVolume", 1f);

        foreach (Sound t in themes) { t.source.volume = themeVolume; }

        currentTheme.source.volume = themeVolume;

        PlayerPrefs.SetFloat("ThemeVolume", themeVolume);
    }
}
