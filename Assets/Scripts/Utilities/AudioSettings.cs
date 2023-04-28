using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections;

public class AudioSettings : MonoBehaviour
{
    public AudioManager audioManager;
    public Slider sfxSlider;
    public Slider themeSlider;

    private const string SFX_VOLUME_KEY = "sfx_volume";
    private const string THEME_VOLUME_KEY = "theme_volume";

    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        sfxSlider = ComponentFinder.FindComponent<Slider>("SFXSlider");
        themeSlider = ComponentFinder.FindComponent<Slider>("ThemeSlider");
        

        //sfxSlider = GameObject.Find("SFXSlider").GetComponent<Slider>();
        //themeSlider = GameObject.Find("ThemeSlider").GetComponent<Slider>();

        // Load the saved volume levels
        //sfxSlider.value = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
        themeSlider.value = PlayerPrefs.GetFloat(THEME_VOLUME_KEY, 1f);

        // Update the volume levels
        //audioManager.SetSFXVolume(sfxSlider.value);
        //audioManager.SetThemeVolume(themeSlider.value);

        // Add listeners to the sliders
        //sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        themeSlider.onValueChanged.AddListener(SetThemeVolume);
    }

    // Set the SFX volume level
    public void SetSFXVolume(float volume)
    {
        audioManager.SetSFXVolume(volume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
    }

    // Set the theme volume level
    public void SetThemeVolume(float volume)
    {
        audioManager.SetThemeVolume(volume);
        PlayerPrefs.SetFloat(THEME_VOLUME_KEY, volume);
    }
}
