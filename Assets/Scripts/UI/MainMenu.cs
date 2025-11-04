using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Audio;
using UnityEngine.UI;

//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Slider musicSlider;
    public Slider sfxSlider;
    private void Start()
    {
        LoadVolume();
        SoundManager.Instance.PlayMusic("Edris' Scene");
    }
    public void PlayGame()
    {

    }

    public void GoToSceneLevelOne()              //This is attached to the button in unity
    {
        SceneManager.LoadScene("StartingCutscene");  //RMB TO UPDATE (NAME) IF SCENE NAME CHANGES
    }

    public void QuitGame()
    {
        Application.Quit();     //To leave the game
    } 

    // This Section below is all for the Music and Sound Setting volume functions.
    public void UpdateMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume); // in the settings, to adjust the volume of the MusicVolume
    }
    public void UpdateSoundVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);   // in the settings, to adjust the volume of the SFXVolume
    }

    // to save the volume setting
    public void SaveVolume()
    {
        audioMixer.GetFloat("MusicVolume", out float musicVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);

        audioMixer.GetFloat("SFXVolume", out float sfxVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    // To Load the Volume setting previously to the sliders
    public void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
    }
}
