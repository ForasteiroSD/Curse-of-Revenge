using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public AudioMixer mainMixer;
    public AudioMixer SFXMixer;
    public void IniciarJogo()
    {
        SceneManager.LoadScene("Game");
    }

    public void SairJogo()
    {
        Application.Quit();
    }

    public void MudarVolumeMusica(float volume)
    {
        mainMixer.SetFloat("MainVolume", volume);
    }
    
    public void MudarVolumeSFX(float volume)
    {
        SFXMixer.SetFloat("MainVolume", volume);
    }

    public void MenuPrincipal()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
