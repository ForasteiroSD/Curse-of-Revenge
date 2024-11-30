using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public AudioMixer mainMixer;
    public AudioMixer SFXMixer;

    private void Start() {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0);

        mainMixer.SetFloat("MainVolume", musicVolume);
        SFXMixer.SetFloat("MainVolume", sfxVolume);

        Transform configsMenu = GameObject.Find("Menu").transform.Find("ConfigsPanel");
        configsMenu.Find("Música").transform.Find("VolumeSlider").GetComponent<Slider>().value = musicVolume;
        configsMenu.Find("FX").transform.Find("VolumeSlider").GetComponent<Slider>().value = sfxVolume;
    }

    public void IniciarJogo()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void SairJogo()
    {
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

}
