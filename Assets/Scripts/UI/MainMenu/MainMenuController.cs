using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public AudioMixer mainMixer;
    public AudioMixer SFXMixer;
    private AudioSource SFXAudioSource;
    [SerializeField] private AudioClip buttonClickSound;

    private void Start() {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0);

        mainMixer.SetFloat("MainVolume", musicVolume);
        SFXMixer.SetFloat("MainVolume", sfxVolume);

        Transform configsMenu = GameObject.Find("Menu").transform.Find("ConfigsPanel");
        configsMenu.Find("Música").transform.Find("VolumeSlider").GetComponent<Slider>().value = musicVolume;
        configsMenu.Find("FX").transform.Find("VolumeSlider").GetComponent<Slider>().value = sfxVolume;

        SFXAudioSource = GameObject.Find("UI").GetComponents<AudioSource>()[1];
    }

    public void IniciarJogo()
    {
        PlayAudio();
        GameManager _gameManager = FindFirstObjectByType<GameManager>();
        StartCoroutine(_gameManager.LoadScene(_gameManager._level));
    }

    public void SairJogo()
    {
        PlayAudio();
        Application.Quit();
    }

    public void MudarVolumeMusica(float volume)
    {
        mainMixer.SetFloat("MainVolume", volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }
    
    public void MudarVolumeSFX(float volume)
    {
        SFXMixer.SetFloat("MainVolume", volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    public void MenuPrincipal()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayAudio()
    {
        SFXAudioSource.PlayOneShot(buttonClickSound);
    }
}
