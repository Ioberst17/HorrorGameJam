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

        SFXInitializationCheck();
        ThemeInitializationCheck();
    }

    void SFXInitializationCheck()
    {
        if (sfxSlider == null) { Debug.Log("Add a game object named 'SFXSlider' to the scene that has a slider component. Note the capitals. It will be used as a slider for the player to control from the pause menu options."); }
        else
        {
            sfxSlider.value = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
            //audioManager.SetSFXVolume(sfxSlider.value);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    void ThemeInitializationCheck()
    {
        if (themeSlider == null) { Debug.Log("Add a game object named 'ThemeSlider' to the scene that has a slider component. Note the capitals. It will be used as a slider for the player to control from the pause menu options."); }
        else
        {
            themeSlider.value = PlayerPrefs.GetFloat(THEME_VOLUME_KEY, 1f);
            //audioManager.SetThemeVolume(themeSlider.value);
            themeSlider.onValueChanged.AddListener(SetThemeVolume);
        }
    }

    // Set the SFX volume level
    public void SetSFXVolume(float volume)
    {
        if(sfxSlider != null)
        {
            audioManager.SetSFXVolume(volume);
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
        }
    }

    // Set the theme volume level
    public void SetThemeVolume(float volume)
    {
        if (themeSlider != null)
        {
            audioManager.SetThemeVolume(volume);
            PlayerPrefs.SetFloat(THEME_VOLUME_KEY, volume);
        }
    }
}
