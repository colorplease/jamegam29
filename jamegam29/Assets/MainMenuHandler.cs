using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    public string mainGameSceneName;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        LoadSettings();

        // Assuming the methods are in the same script as the sliders
        musicSlider.onValueChanged.AddListener(delegate { SaveSettings(musicSlider.value, sfxSlider.value); });
        sfxSlider.onValueChanged.AddListener(delegate { SaveSettings(musicSlider.value, sfxSlider.value); });
    }

    public void ButtonEvt_Play()
    {
        SceneManager.LoadScene(mainGameSceneName);
    }

    public void ButtonEvt_Exit()
    {
        Application.Quit();
    }
    
    private void SaveSettings(float musicVolume, float sfxVolume)
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save(); 
    }

    private void LoadSettings()
    {
        // PlayerPrefs.GetFloat returns 0 if "MusicVolume" or "SFXVolume" doesn't exist
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f); // 1f is default value
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f); // 1f is default value

        // Apply the loaded values to your sliders or audio mixer here. 
        // Assuming you have references to your sliders or audio mixer.
        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;
    }
}
