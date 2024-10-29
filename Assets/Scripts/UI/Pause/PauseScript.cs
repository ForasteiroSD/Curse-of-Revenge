using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        Time.timeScale = 0f;
    }

    public void VoltarJogo()
    {
        EventSystem.current.SetSelectedGameObject(null);
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
