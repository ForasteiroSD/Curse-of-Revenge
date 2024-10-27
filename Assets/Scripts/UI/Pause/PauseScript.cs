using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    public AudioMixer mainMixer;
    public GameObject pauseMenu;
    public GameObject configsMenu;

    public bool isPaused;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                VoltarJogo();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        configsMenu.SetActive(false);
        Time.timeScale = 0f;
    }

    public void VoltarJogo()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Sair()
    {
        Application.Quit();
    }
    
    public void MudarVolume(float volume)
    {
        mainMixer.SetFloat("MainVolume", volume);
    }
}
