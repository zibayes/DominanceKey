using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;
    public float volume;

    public void Start()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
            LoadMusicVolume();
        if (PlayerPrefs.HasKey("SFXVolume"))
            LoadMusicVolume();
        SetMusicVolume();
        SetSFXVolume();

        if (PlayerPrefs.HasKey("QualityLevel"))
            LoadQuality();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
    }

    public void LoadQuality()
    {
        int qualityIndex = PlayerPrefs.GetInt("QualityLevel");
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetMusicVolume()
    {
        volume = musicSlider.value;
        audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume()
    {
        volume = sfxSlider.value;
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void LoadMusicVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        SetMusicVolume();
    }

    public void LoadSFXVolume()
    {
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        SetSFXVolume();
    }
}
