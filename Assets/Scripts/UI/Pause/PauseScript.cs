using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    public AudioMixer mainMixer;
    public GameObject pauseMenu;
    public GameObject configsMenu;

    public bool isPaused;
    
    void Start()
    {
        pauseMenu.SetActive(false);
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
    }

    public void Sair()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
    
    public void MudarVolume(float volume)
    {
        mainMixer.SetFloat("MainVolume", volume);
    }
}
