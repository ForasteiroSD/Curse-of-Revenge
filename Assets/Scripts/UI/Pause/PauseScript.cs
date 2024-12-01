using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{
    public AudioMixer mainMixer;
    public AudioMixer SFXMixer;
    public GameObject pauseMenu;
    public GameObject configsMenu;
    private AudioManager audioManager;

    public bool isPaused;
    
    void Start()
    {
        pauseMenu.SetActive(false);

        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0);

        mainMixer.SetFloat("MainVolume", musicVolume);
        SFXMixer.SetFloat("MainVolume", sfxVolume);

        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

        Transform configsMenu = GameObject.Find("UI").transform.Find("PausePanel").Find("ConfigsPanel");
        configsMenu.Find("Música").transform.Find("VolumeSlider").GetComponent<Slider>().value = musicVolume;
        configsMenu.Find("FX").transform.Find("VolumeSlider").GetComponent<Slider>().value = sfxVolume;
    }

    public void OnSubmit(InputValue value)
    {
        if (value.isPressed)
        {
            PauseGame();
        }
    }
    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        configsMenu.SetActive(false);
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void VoltarJogo()
    {
        EventSystem.current.SetSelectedGameObject(null);
        pauseMenu.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;
        PlayAudio();
    }

    public void Sair()
    {
        PlayAudio();
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
    
    public void MudarVolume(float volume)
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

    public void PlayAudio()
    {
        audioManager.TocarSFX(31);
    }
}
