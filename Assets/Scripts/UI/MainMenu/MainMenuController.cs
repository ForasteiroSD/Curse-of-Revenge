using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public AudioMixer mainMixer;
    public void IniciarJogo()
    {
        SceneManager.LoadScene("BrenoCena");
    }

    public void SairJogo()
    {
        Application.Quit();
    }

    public void MudarVolume(float volume)
    {
        mainMixer.SetFloat("MainVolume", volume);
    }

}